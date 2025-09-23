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
using UnrealEssentials.Types;
using UnrealEssentials.Unreal;
using UTOC.Stream.Emulator.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
            _signatures.TocVersion, _signatures.PakVersion,
            _signatures.FileIoStoreOpenContainer, _signatures.ReadBlocks, 
            AddPakFolder, RemovePakFolder
            );
    }

    internal static unsafe bool TryCreateContext(IModLoader _modLoader, IModConfig _modConfig, out Context? context)
    {
        context = null;
        // Setup mods path
        var modPath = new DirectoryInfo(_modLoader.GetDirectoryForModId(_modConfig.ModId)).Parent!.FullName;
        if (!TryGetSignatures(_modLoader, out var sigs)) return false;
        context = new(Native.FPakSigningKeys.NewBlank(), modPath, DoesGameUseUtocs(sigs), sigs, _modLoader);
        return true;
    }

    internal void AddFolder(string folder)
    {
        if (!Directory.Exists(folder))
        {
            LogError($"Folder {folder} does not exist, skipping.");
            return;
        }
        _pakFolders.Add(folder);
        AddRedirections(folder, null);
        Log($"Loading files from {folder}");

        // Prevent UTOC Emulator from wasting time creating UTOCs if the game doesn't use them
        if (_hasUtocs)
            _utocEmulator.AddFromFolder(folder);
    }

    internal void AddFolderWithVirtualMount(string folder, string virtualPath)
    {
        if (!Directory.Exists(folder))
        {
            LogError($"Folder {folder} does not exist, skipping.");
            return;
        }
        _pakFolders.Add(folder);
        AddRedirections(folder, virtualPath);
        Log($"Loading files from {folder}, with emulated mountFilePath {virtualPath}");

        // Prevent UTOC Emulator from wasting time creating UTOCs if the game doesn't use them
        if (_hasUtocs)
            _utocEmulator.AddFromFolderWithMount(folder, virtualPath);
    }

    internal void AddFileWithVirtualMount(string file, string virtualPath)
    {
        if(!File.Exists(file))
        {
            LogError($"File {file} does not exist, skipping.");
            return;
        }
        _pakFolders.Add(file);
        _redirections[virtualPath] = file;
        Log($"Loading file at {file}, with emulated mountFilePath {virtualPath}");

        // Prevent UTOC Emulator from wasting time creating UTOCs if the game doesn't use them
        if (_hasUtocs)
            _utocEmulator.AddFromFolderWithMount(file, virtualPath);
    }

    internal void AddRedirections(string modsPath, string? virtualPath)
    {
        foreach (var file in Directory.EnumerateFiles(modsPath, "*", SearchOption.AllDirectories))
        {
            string relativeFilePath = Path.GetRelativePath(modsPath, file);
            string gamePath;

            if (!string.IsNullOrWhiteSpace(virtualPath))
            {
                // Use virtual mount mountFilePath
                gamePath = Path.Combine(@"..\..\..", virtualPath, relativeFilePath);
            }
            else
            {
                gamePath = Path.Combine(@"..\..\..", relativeFilePath);
            }

            string normalizedGamePath = gamePath.Replace('\\', '/');
            _redirections[gamePath] = file;
            _redirections[normalizedGamePath] = file;
        }
    }
    
    private void AddPakFolder(string path)
    {
        _pakFolders.Add(path);
        AddRedirections(path, null);
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

    private static bool TryGetSignatureFromFileDescription(Dictionary<string?, Signatures> VersionSigs, ProcessModule mainModule, out Signatures? sigs)
    {
        // Dynamically load DLL needed for methods to get executable resource metadata from
        sigs = null;
        var winVerDll = Imports.LoadLibraryA("Api-ms-win-core-version-l1-1-0.dll");
        if (winVerDll == null)
        {
            return false;
        }
        unsafe
        {
            var getFileVersionInfoSizeA =
                (delegate* unmanaged[Stdcall]<string, uint*, uint>)Imports.GetProcAddress(winVerDll,
                    "GetFileVersionInfoSizeA");
            if (getFileVersionInfoSizeA == null)
            {
                return false;
            }
            var infoSize = getFileVersionInfoSizeA(mainModule!.FileName, null);
            var infoBuffer = new byte[infoSize];
            var getFileVersionInfoA =
                (delegate* unmanaged[Stdcall]<string, uint, uint, byte*, bool>)Imports.GetProcAddress(winVerDll,
                    "GetFileVersionInfoA");
            if (getFileVersionInfoA == null)
            {
                return false;
            }
            fixed (byte* pInfoBuffer = infoBuffer)
            {
                if (!getFileVersionInfoA(mainModule!.FileName, 0, infoSize, pInfoBuffer))
                {
                    return false;
                }
                // https://learn.microsoft.com/en-us/windows/win32/api/winver/nf-winver-verqueryvaluea
                var verQueryValueA =
                    (delegate* unmanaged[Stdcall]<byte*, string, nint*, uint*, bool>)Imports.GetProcAddress(winVerDll,
                        "VerQueryValueA");
                if (verQueryValueA == null)
                {
                    return false;
                }

                // Get language + codepage
                LanguageCodePage* translate = null;
                uint translateSize = 0;
                if (!verQueryValueA(pInfoBuffer, "\\VarFileInfo\\Translation", (nint*)(&translate), &translateSize))
                {
                    return false;
                }

                for (int i = 0; i < translateSize / sizeof(LanguageCodePage); i++)
                {
                    // Check FileDescription entry for StringFileInfo
                    var translateEntry = (translate + i);
                    char* fileDescription = null;
                    uint fileDescBytes = 0;
                    // VerQueryValue for strings includes null terminator in length
                    if (!verQueryValueA(pInfoBuffer,
                            $"\\StringFileInfo\\{translateEntry->wLanguage:x04}{translateEntry->wCodePage:x04}\\FileDescription", 
                            (nint*)(&fileDescription), &fileDescBytes))
                    {
                        return false;
                    }
                    if (VersionSigs.TryGetValue(Marshal.PtrToStringAnsi((nint)fileDescription, (int)fileDescBytes - 1), 
                            out var sigsMaybe))
                    {
                        sigs = sigsMaybe;
                        return true;
                    }
                }
            }
        }
        return false;
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
                    ((ISignatureList)x.GetConstructor( BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, [], null).Invoke([])).GetSignatures()
                )
            ).ToDictionary(x => x.Item1, x => x.Item2);
        // Try and find based on file name
        if (VersionSigs.TryGetValue(fileName, out sigs))
            return true;

        // Try and find based on the executable's file description
        if (TryGetSignatureFromFileDescription(VersionSigs, mainModule!, out var sigsMaybe))
        {
            sigs = sigsMaybe.Value;
            return true;
        }
        
        _modLoader.GetController<IScannerFactory>().TryGetTarget(out var scannerFactory);
        var scanner = scannerFactory!.CreateScanner(CurrentProcess, mainModule);

        // Try and find based on branch name
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

    internal void LoadUEMounts(string modRootPath, string mountFilePath)
    {
        if (File.Exists(mountFilePath))
        {
            Log($"Loading virtual paths from {mountFilePath}.");
            List<VirtualEntry> virtualPaths = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance).WithEnforceRequiredMembers()
            .Build().Deserialize<List<VirtualEntry>>(File.ReadAllText(mountFilePath));
            foreach (var item in virtualPaths)
            {
                if (File.Exists(item.OSPath))
                {
                    AddFileWithVirtualMount(Path.Combine(modRootPath, item.OSPath), item.VirtualPath);
                }
                else if (Directory.Exists(item.OSPath))
                {
                    AddFolderWithVirtualMount(Path.Combine(modRootPath, item.OSPath), item.VirtualPath);
                }
                else
                {
                    LogError($"OSPath: {item.OSPath} supplied in {mountFilePath} does not exist!");
                }
            }
        }
    }
}