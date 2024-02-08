using System.Runtime.InteropServices;
using UTOC.Stream.Emulator.Interfaces;
using static UnrealEssentials.Unreal.UnrealArray;
using static UnrealEssentials.Unreal.UnrealString;

namespace UnrealEssentials.Unreal;
internal unsafe class Native
{
    /// <summary>
    /// This isn't neccessarily accurate to Unreal Engine source, 
    /// it's just good enough for removing signatures
    /// </summary>
    internal struct FPakSigningKeys
    {
        internal nuint Function;
        internal int Size;
    }

    internal struct FIoStoreEnvironment
    {
        internal FString Path;
        internal int Order;
    }

    internal delegate FPakSigningKeys* GetPakSigningKeysDelegate();
    internal delegate void GetPakFoldersDelegate(nuint cmdLine, TArray<FString>* outPakFolders);
    internal delegate int GetPakOrderDelegate(FString* PakFilePath);
    internal delegate nuint PakOpenReadDelegate(nuint thisPtr, nint fileNamePtr, bool bAllowWrite);
    internal delegate nuint PakOpenAsyncReadDelegate(nint thisPtr, nint fileNamePtr);
    internal delegate bool IsNonPakFilenameAllowedDelegate(nuint thisPtr, FString* Filename);
    internal delegate bool FileExistsDlegate(nuint thisPtr, char* Filename);
    internal delegate nuint FIoBatchReadInternal(nuint thisPtr, FIoChunkId* ioChunkIdPtr, nuint ioReadOptionsPtr, int priority);
    internal delegate nuint FIoStoreTocResurceRead(nuint retStorage, nint tocFilePath, int readOptions, nuint outTocResource);
}
