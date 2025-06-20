using UnrealEssentials.Interfaces;

namespace UnrealEssentials;
public unsafe class Api : IUnrealEssentials
{
    private Action<string> _addFolder;
    private Action<string, string> _addFolderWithMount;

    internal Api(Action<string> addFolder, Action<string, string> addFolderWithMount)
    {
        _addFolder = addFolder;
        _addFolderWithMount = addFolderWithMount;
    }

    public void AddFromFolder(string path) => _addFolder(path);

    public void AddFromFolderWithMount(string path, string virtualPath)
        => _addFolderWithMount(path, virtualPath);

    public IUnrealMemory GetMemory() => Mod.Memory;
}
