using UTOC.Stream.Emulator.Interfaces;

namespace UnrealEssentials;

/*
public enum IoStoreLoadingThreadType
{
    UE4,
    UE5
}
*/
public enum StartLoadingDelegateType
{
    NoArgs, // UE 4.25-4.27
    AddIoBatch, // UE 5.0
    PackageNodeArray, // UE 5.1
    AddThreadState, // UE 5.2-5.3
    DescAddInstancingContext, // UE 5.4-5.6
    AsyncPackageInheritsRefCount, // UE 5.7+
}

public struct Signatures
{
    internal string GetPakSigningKeys { get; set; } // Function call to FCoreDelegates::GetPakSigningKeysDelegate in FIoStoreTocResource::Read (short jump)
    internal string GetPakFolders { get; set; } // FPakPlatformFile::GetPakFolders
    internal string GMalloc { get; set; } // during initializing GMalloc. Long Jump
    internal string GetPakOrder { get; set; } // FPakPlatformFile::GetPakOrderFromPakFilePath
    internal string PakOpenRead { get; set; } // FPakPlatformFile::OpenRead
    internal string PakOpenAsyncRead { get; set; } // FPakPlatformFile::OpenAsyncRead
    internal string IsNonPakFilenameAllowed { get; set; } // FPakPlatformFile::IsNonPakFilenameAllowed
    internal string FileIoStoreOpenContainer { get; set; } // FGenericFileIoStoreImpl::OpenContainer
    internal string ReadBlocks { get; set; } // FFileIoStore::ReadBlocks
    internal TocType? TocVersion { get; set; }
    internal PakType PakVersion { get; set; }
    internal TocChunkIdType ChunkIdType { get; set; }
    internal string FileExists { get; set; } // FPakPlatformFile::FileExists
    internal string FIOBatch_ReadInternal { get; set; }
    internal string FAsyncPackage2_StartLoading { get; set; }
    internal StartLoadingDelegateType StartLoadDelegate { get; set; }
    internal string GFNamePool { get; set; }
}
