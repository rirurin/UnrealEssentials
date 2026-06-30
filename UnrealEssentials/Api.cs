using UnrealEssentials.Components;
using UnrealEssentials.Interfaces;
using UTOC.Stream.Emulator.Interfaces;

namespace UnrealEssentials;
public unsafe class Api : IUnrealEssentials
{
    private Action<string> _addFolder;
    private Action<string, string> _addFolderWithVirtualMount;
    private Action<string, string> _addFileWithVirtualMount;
    private EngineVersion _version;

    internal Api(EngineVersion version, Action<string> addFolder, Action<string, string> addFolderWithVirtualMount, 
        Action<string, string> addFileWithVirtualMount)
    {
        _version = version;
        _addFolder = addFolder;
        _addFolderWithVirtualMount = addFolderWithVirtualMount;
        _addFileWithVirtualMount = addFileWithVirtualMount;
    }

    public void AddFromFolder(string path) => _addFolder(path);

    public void AddFromFolderWithVirtualMount(string path, string virtualPath) => _addFolderWithVirtualMount(path, virtualPath);

    public void AddFileWithVirtualMount(string path, string virtualPath) => _addFileWithVirtualMount(path, virtualPath);

    public IUnrealMemory GetMemory() => Mod.Memory;

    public string GetEngineVersion() => _version.ToBranchVersion();
}
