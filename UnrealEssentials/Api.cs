using UnrealEssentials.Interfaces;
using UnrealEssentials.Unreal;

namespace UnrealEssentials;
public unsafe class Api : IUnrealEssentials
{

    private Action<string> _addFolder;

    internal Api(Action<string> addFolder)
    {
        _addFolder = addFolder;
    }

    public void AddFromFolder(string path) => _addFolder(path);
    public IUnrealMemory GetMemory() => Mod.Memory;
}
