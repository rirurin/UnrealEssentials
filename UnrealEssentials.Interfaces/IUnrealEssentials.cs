namespace UnrealEssentials.Interfaces;

/// <summary>
/// The base interface for the UnrealEssentials API
/// </summary>
public interface IUnrealEssentials
{
    /// <summary>
    /// Adds files from the folder at <paramref name="path"/> 
    /// This folder is treated like it was the UnrealEssentials folder inside of a mod
    /// </summary>
    /// <param name="path">Path to the folder that contains files to be loaded</param>
    void AddFromFolder(string path);

    void AddFromFolderWithMount(string folderPath, string virtualPath);

    /// <summary>
    /// Gets the <see cref="IUnrealMemory"/> instance.
    /// This can be used to manipulate memory with the native Unreal Engine functions.  
    /// </summary>
    /// <returns>The <see cref="IUnrealMemory"/> instance</returns>
    /// <remarks>The returned instance is a singleton so you can keep a reference to it rather than calling this every time you need it.</remarks>
    IUnrealMemory GetMemory();
}