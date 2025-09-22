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

    /// <summary>
    /// Adds files from the folder at <paramref name="folderPath"/> 
    /// This folder is treated like it was the UnrealEssentials folder inside of a mod, all files are assumed to be under the virtual path provided.
    /// </summary>
    /// <param name="folderPath">Path to the folder that contains files to be loaded</param>
    /// <param name="virtualPath">The virtual path to mount the folder's files at, the virtual path must begin with either 'GameName'/Content or Engine</param>
    void AddFromFolderWithVirtualMount(string folderPath, string virtualPath);

    /// <summary>
    /// Adds files from the folder at <paramref name="filePath"/> 
    /// This file will be added via UnrealEssentials and mounted at the virtual path provided.
    /// </summary>
    /// <param name="filePath">Path to the file to be loaded</param>
    /// <param name="virtualPath">The virtual path to mount the file at, the virtual path must begin with either 'GameName'/Content or Engine</param>
    void AddFileWithVirtualMount(string filePath, string virtualPath);

    /// <summary>
    /// Gets the <see cref="IUnrealMemory"/> instance.
    /// This can be used to manipulate memory with the native Unreal Engine functions.  
    /// </summary>
    /// <returns>The <see cref="IUnrealMemory"/> instance</returns>
    /// <remarks>The returned instance is a singleton so you can keep a reference to it rather than calling this every time you need it.</remarks>
    IUnrealMemory GetMemory();
}