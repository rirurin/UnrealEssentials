using UnrealEssentials.Interfaces;

namespace UnrealEssentials;
public unsafe class Api : IUnrealEssentials
{
    private Action<string> _addFolder;
    private Action<string, string> _addFolderWithVirtualMount;
    private Action<string, string> _addFileWithVirtualMount;

    internal Api(Action<string> addFolder, Action<string, string> addFolderWithVirtualMount, Action<string, string> addFileWithVirtualMount)
    {
        _addFolder = addFolder;
        _addFolderWithVirtualMount = addFolderWithVirtualMount;
        _addFileWithVirtualMount = addFileWithVirtualMount;
    }

    public void AddFromFolder(string path) => _addFolder(path);

    public void AddFromFolderWithVirtualMount(string path, string virtualPath) => _addFolderWithVirtualMount(path, virtualPath);

    public void AddFileWithVirtualMount(string path, string virtualPath) => _addFileWithVirtualMount(path, virtualPath);

    public IUnrealMemory GetMemory() => Mod.Memory;
}
