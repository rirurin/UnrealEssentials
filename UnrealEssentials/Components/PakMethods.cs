using Reloaded.Hooks.Definitions;
using System.Runtime.InteropServices;
using UnrealEssentials.Configuration;
namespace UnrealEssentials.Components;
using static Utils;
using static Unreal.UnrealArray;
using static Unreal.UnrealString;

internal class PakMethods
{
    // State objects
    private readonly IReloadedHooks _hooks;
    private Config _configuration;
    private Context _context;
   
    // Function hooks
    private IHook<GetPakFolders>? _getPakFoldersHook;
    private IHook<GetPakOrder>? _getPakOrderHook;
    private IHook<PakOpenRead>? _pakOpenReadHook;
    private IHook<PakOpenAsyncRead>? _pakOpenAsyncReadHook;
    private IHook<IsNonPakFilenameAllowed>? _isNonPakFilenameAllowedHook;
    private IHook<FileExists>? _fileExistsHook;
    
    public unsafe PakMethods(IReloadedHooks hooks, Config configuration, Context context)
    {
        // Get dependency objects
        _hooks = hooks;
        _configuration = configuration;
        _context = context;
        // Load files from our mod
        SigScan(context._signatures.GetPakFolders, "GetPakFolders", address => _getPakFoldersHook = _hooks.CreateHook<GetPakFolders>(GetPakFoldersImpl, address).Activate());
        // Fix priority
        SigScan(context._signatures.GetPakOrder, "GetPakOrder", address => _getPakOrderHook = _hooks.CreateHook<GetPakOrder>(GetPakOrderImpl, address).Activate());
        // Allow loose pak loading
        SigScan(context._signatures.PakOpenRead, "PakOpenRead", address => _pakOpenReadHook = _hooks.CreateHook<PakOpenRead>(PakOpenReadImpl, address).Activate());
        SigScan(context._signatures.PakOpenAsyncRead, "PakOpenAsyncRead", address => _pakOpenAsyncReadHook = _hooks.CreateHook<PakOpenAsyncRead>(PakOpenAsyncReadImpl, address).Activate());
        SigScan(context._signatures.IsNonPakFilenameAllowed, "IsNonPakFilenameAllowed", address => _isNonPakFilenameAllowedHook = _hooks.CreateHook<IsNonPakFilenameAllowed>(IsNonPakFilenameAllowedImpl, address).Activate());
        SigScan(context._signatures.FileExists, "FileExists", address => _fileExistsHook = _hooks.CreateHook<FileExists>(FileExistsImpl, address).Activate());
    }
    
    internal unsafe delegate void GetPakFolders(nuint cmdLine, TArray<FString>* outPakFolders);
    private unsafe void GetPakFoldersImpl(nuint cmdLine, TArray<FString>* outPakFolders)
    {
        _getPakFoldersHook!.OriginalFunction(cmdLine, outPakFolders);

        // Resize the array
        if (outPakFolders->Capacity <= _context!._pakFolders.Count + outPakFolders->Length)
        {
            outPakFolders->Resize(_context!._pakFolders.Count + outPakFolders->Length);
        }

        // Add files from mods
        foreach (var pakFolder in _context!._pakFolders)
        {
            var str = new FString(pakFolder);
            outPakFolders->Add(str);
        }
    }
    internal unsafe delegate int GetPakOrder(FString* PakFilePath);
    private unsafe int GetPakOrderImpl(FString* PakFilePath)
    {
        // TODO write/copy Contains and StartsWith functions that use the FString* directly
        // instead of making it a string each time (StartsWith is probably much more important)
        var path = PakFilePath->ToString();

        // A vanilla file, use normal order
        if (!path.StartsWith(_context._modsPath))
            return _getPakOrderHook!.OriginalFunction(PakFilePath);

        // One of our files, override order
        for (int i = 0; i < _context!._pakFolders.Count; i++)
        {
            if (path.Contains(_context!._pakFolders[i]))
            {
                LogDebug($"Set order of {path} to {(i + 1) * 1000}");
                return (i + 1) * 10000;
            }
        }

        // This shouldn't happen...
        LogError($"Unable to decide order for {path}. This shouldn't happen!");
        return 0;
    }
    internal delegate nuint PakOpenRead(nuint thisPtr, nint fileNamePtr, bool bAllowWrite);
    private nuint PakOpenReadImpl(nuint thisPtr, nint fileNamePtr, bool bAllowWrite)
    {
        var fileName = Marshal.PtrToStringUni(fileNamePtr);
        if (_configuration.FileAccessLog)
        {
            Log($"Opening: {fileName}");
        }

        // No loose file, vanilla behaviour
        if (!_context.TryFindLooseFile(fileName, out var looseFile))
            return _pakOpenReadHook!.OriginalFunction(thisPtr, fileNamePtr, bAllowWrite);

        // Get the pointer to the loose file that UE wants
        Log($"Redirecting {fileName} to {looseFile}");
        var looseFilePtr = Marshal.StringToHGlobalUni(looseFile);
        var res = _pakOpenReadHook!.OriginalFunction(thisPtr, looseFilePtr, bAllowWrite);

        // Clean up
        Marshal.FreeHGlobal(looseFilePtr);
        return res;
    }
    
    internal delegate nuint PakOpenAsyncRead(nint thisPtr, nint fileNamePtr);
    private nuint PakOpenAsyncReadImpl(nint thisPtr, nint fileNamePtr)
    {
        var fileName = Marshal.PtrToStringUni(fileNamePtr);
        if (_configuration.FileAccessLog)
        {
            Log($"Opening async: {fileName}");
        }

        // No loose file, vanilla behaviour
        if (!_context.TryFindLooseFile(fileName, out var looseFile))
            return _pakOpenAsyncReadHook!.OriginalFunction(thisPtr, fileNamePtr);

        // Get the pointer to the loose file that UE wants
        Log($"Redirecting async {fileName} to {looseFile}");
        var looseFilePtr = Marshal.StringToHGlobalUni(looseFile);
        var res = _pakOpenAsyncReadHook!.OriginalFunction(thisPtr, looseFilePtr);

        // Clean up
        //Marshal.FreeHGlobal(looseFilePtr);
        return res;
    }
    internal unsafe delegate bool IsNonPakFilenameAllowed(nuint thisPtr, FString* Filename);
    private unsafe bool IsNonPakFilenameAllowedImpl(nuint thisPtr, FString* Filename)
    {
        return true;
    }
    internal unsafe delegate bool FileExists(nuint thisPtr, char* Filename);
    private unsafe bool FileExistsImpl(nuint thisPtr, char* Filename)
    {
        var fileName = Marshal.PtrToStringUni((nint)Filename);

        if (_context.TryFindLooseFile(fileName, out _))
            return true;

        return _fileExistsHook!.OriginalFunction(thisPtr, Filename);
    }
}