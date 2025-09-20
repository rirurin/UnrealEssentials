using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan.Definitions;
using Reloaded.Mod.Interfaces;
using UnrealEssentials.Configuration;
using UnrealEssentials.SignatureList;
using UnrealEssentials.SignatureList.Game;
using UnrealEssentials.Unreal;
using UTOC.Stream.Emulator.Interfaces;

namespace UnrealEssentials.Components;
using static Utils;

internal class Context
{
    internal unsafe Native.FPakSigningKeys* _signingKeys { get;}
    internal string _modsPath { get; }
    internal List<string> _pakFolders = new();
    internal Dictionary<string, string> _redirections { get; } = new();
    internal IUtocEmulator? _utocEmulator;
    internal bool _hasUtocs { get; }
    internal Signatures _signatures { get; }
    
    internal unsafe Context(Native.FPakSigningKeys* signingKeys, string modsPath, bool hasUtocs, Signatures signatures, IModLoader _modLoader)
    {
        _signingKeys = signingKeys;
        _modsPath = modsPath;
        _hasUtocs = hasUtocs;
        _signatures = signatures;
       
        // Initialize UTOC Emulator
        _modLoader.GetController<IUtocEmulator>().TryGetTarget(out _utocEmulator);
        _utocEmulator!.Initialise(
            _signatures.TocVersion, _signatures.PakVersion, _signatures.ChunkIdType, 
            _signatures.FileIoStoreOpenContainer, _signatures.ReadBlocks, 
            AddPakFolder, RemovePakFolder
            );
    }

    internal static unsafe Context? TryCreateContext(IModLoader _modLoader, IModConfig _modConfig)
    {
        // Setup mods path
        var modPath = new DirectoryInfo(_modLoader.GetDirectoryForModId(_modConfig.ModId)).Parent!.FullName;
        if (!TryGetSignatures(_modLoader, out var sigs)) return null;
        return new(Native.FPakSigningKeys.NewBlank(), modPath, DoesGameUseUtocs(sigs), sigs, _modLoader);
    }
    
    internal void AddFolder(string folder)
    {
        _pakFolders.Add(folder);
        AddRedirections(folder);
        Log($"Loading files from {folder}");

        // Prevent UTOC Emulator from wasting time creating UTOCs if the game doesn't use them
        if (_hasUtocs)
            _utocEmulator!.AddFromFolder(folder);
    }

    private void AddRedirections(string modsPath)
    {
        foreach (var file in Directory.EnumerateFiles(modsPath, "*", SearchOption.AllDirectories))
        {
            var gamePath = Path.Combine(@"..\..\..", Path.GetRelativePath(modsPath, file)); // recreate what the game would try to load
            _redirections[gamePath] = file;
            _redirections[gamePath.Replace('\\', '/')] = file; // UE could try to load it using either separator
        }
    }
    
    private void AddPakFolder(string path)
    {
        _pakFolders.Add(path);
        AddRedirections(path);
        Log($"Loading PAK files from {path}");
    }

    private void RemovePakFolder(string path)
    {
        if (_pakFolders.Remove(path))
        {
            Log($"Removed pak folder {path}");
        }
    }
    
    internal bool TryFindLooseFile(string gameFilePath, out string? looseFile)
    {
        return _redirections.TryGetValue(gameFilePath, out looseFile);
    }
    
    private static bool TryGetSignatures(IModLoader _modLoader, out Signatures sigs)
    {
        var CurrentProcess = Process.GetCurrentProcess();
        var mainModule = CurrentProcess.MainModule;
        var fileName = Path.GetFileName(mainModule!.FileName);

        var VersionSigs = Assembly.GetExecutingAssembly().GetTypes().Where(x =>
            x.CustomAttributes.Any(a => a.AttributeType == typeof(SignatureAttribute)))
            .Select(x => (
                    // get the string value of VersionIdentifier from the first attribute of name "SignatureAttribute"
                    x.CustomAttributes.Where(a => a.AttributeType == typeof(SignatureAttribute)).Select(a => a.NamedArguments.Where(b => b.MemberName == "VersionIdentifier").Select(b => (string)b.TypedValue.Value).First()).First(),
                    // create an instance of the target class, then get it's signature list
                    ((ISignatureList)x.GetConstructor( BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] {}, null).Invoke(new object[] {})).GetSignatures()
                )
            ).ToDictionary(x => x.Item1, x => x.Item2);
        // Try and find based on file name
        if (VersionSigs.TryGetValue(fileName, out sigs))
            return true;

        // Try and find based on branch name
        _modLoader.GetController<IScannerFactory>().TryGetTarget(out var scannerFactory);
        var scanner = scannerFactory.CreateScanner(CurrentProcess, mainModule);
        var results = scanner.FindPatterns(new List<string>() {
            "2B 00 2B 00 55 00 45 00 34 00 2B 00", // ++UE4+
            "2B 00 2B 00 75 00 65 00 34 00 2B 00", // ++ue4+
            "2B 00 2B 00 55 00 45 00 35 00 2B 00", // ++UE5+
            "2B 00 2B 00 75 00 65 00 35 00 2B 00", // ++ue5+
        }).Where(x => x.Found).ToList();
        if (results.Count == 0)
        {
            LogError($"Unable to find Unreal Engine version number, Unreal Essentials will not work!\n" +
                     $"If this game does not use Unreal Engine please disable Unreal Essentials.\n" +
                     $"If you are sure this is an Unreal Engine game then please report this at github.com/AnimatedSwine37/UnrealEssentials " +
                     $"so support can be added.");
            return false;
        }
        string branch = Marshal.PtrToStringUni(results[0].Offset + BaseAddress)!;
        Log($"Unreal Engine branch is {branch}");
        if (!VersionSigs.TryGetValue(branch, out sigs))
        {
            LogError($"Unable to find signatures for Unreal Engine branch {branch}, Unreal Essentials will not work!\n" +
                "Please report this at github.com/AnimatedSwine37/UnrealEssentials.");
            return false;
        }

        return true;
    }
    
    private static bool DoesGameUseUtocs(Signatures sigs)
    {
        if (sigs.TocVersion == null)
        {
            Log("Game does not use UTOCs as TocVersion was null");
        }

        // Look for any utoc files in the game's folder
        if(Directory.GetFiles("../../..", "*.utoc", SearchOption.AllDirectories).Length == 0)
        {
            Log("Game does not include any UTOC files");
            return false;
        }

        return true;
    }
}