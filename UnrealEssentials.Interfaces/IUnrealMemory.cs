namespace UnrealEssentials.Interfaces;

/// <summary>
/// The interface for managing memory native Unreal Engine functions
/// </summary>
public interface IUnrealMemory
{
    /// <summary>
    /// Allocates a block of memory
    /// </summary>
    /// <param name="count">Number of bytes to allocate</param>
    /// <param name="alignment">Alignment of the allocation</param>
    /// <remarks>
    /// Should only be used once the game has finished launching
    /// </remarks>
    /// <returns>Returns a pointer to the beginning of the new memory block</returns>
    nuint Malloc(nuint count, uint alignment = 0);

    /// <summary>
    /// Similar to <see cref="Malloc"/>, but may return a nullptr result if the allocation request cannot be satisfied
    /// </summary>
    /// <param name="count">Number of bytes to allocate.</param>
    /// <param name="alignment">Alignment of the allocation.</param>
    /// <remarks>
    /// Should only be used once the game has finished launching.
    /// </remarks>
    /// <returns>Returns a pointer to the beginning of the new memory block. If the allocation fails, returns a nullptr</returns>
    nuint TryMalloc(nuint count, uint alignment = 0);

    /// <summary>
    /// Resizes a previously allocated block of memory, preserving its contents
    /// </summary>
    /// <param name="original">Pointer to the original memory.</param>
    /// <param name="count">Number of bytes to allocate.</param>
    /// <param name="alignment">Alignment of the allocation.</param>
    /// <remarks>
    /// Should only be used once the game has finished launching.
    /// </remarks>
    /// <returns>Returns a pointer to the beginning of the new memory block</returns>
    nuint Realloc(nuint original, nuint count, uint alignment = 0);

    /// <summary>
    /// Similar to <see cref="Realloc"/>, but may return a nullptr if the allocation request cannot be satisfied
    /// </summary>
    /// <param name="original">Pointer to the original memory.</param>
    /// <param name="count">Number of bytes to allocate.</param>
    /// <param name="alignment">Alignment of the allocation.</param>
    /// <remarks>
    /// Should only be used once the game has finished launching.
    /// </remarks>
    /// <returns>Returns a pointer to the beginning of the new memory block. If the allocation fails, returns a nullptr</returns>
    nuint TryRealloc(nuint original, nuint count, uint alignment = 0);

    /// <summary>
    /// Deallocates a block of memory
    /// </summary>
    /// <param name="original">Pointer to the original memory.</param>
    /// <remarks>
    /// Should only be used once the game has finished launching.
    /// </remarks>
    void Free(nuint original);

    ///<summary>
    ///If possible, determines the size of the memory allocated at the given address
    /// </summary>
    /// <param name="original">Pointer to memory we are checking the size of</param>
    /// <param name="size">If found, the size of the memory allocated at <paramref name="original"/></param>
    /// <remarks>
    /// Should only be used once the game has finished launching.
    /// </remarks>
    /// <returns>Returns true if this succeeds in determining the size of the memory allocated at the given address</returns>
    bool GetAllocationSize(nuint original, out nuint size);
}