namespace UTOC.Stream.Emulator.Interfaces;

public enum TocType : byte
{
    Initial = 1, // 4.25
    DirectoryIndex = 2, // 4.25+, 4.26
    PartitionSize = 3, // 4.27
    PerfectHash = 4, // 5.0 - 5.3
    OnDemandMetaData = 6, // 5.4
    ReplaceIoChunkHashWithIoHash = 8, // 5.5
}

// UE4 pak type
// See https://github.com/trumank/repak?tab=readme-ov-file#compatibility for more details
public enum PakType
{
    NoTimestamps = 1,
    CompressionEncryption = 2,
    IndexEncryption = 3,
    RelativeChunkOffsets = 4,
    EncryptionKeyGuid = 5,
    FNameBasedCompressionA = 6,
    FNameBasedCompressionB = 7,
    FrozenIndex = 8,
    Fn64BugFix = 9
}

public enum TocChunkIdType : byte
{
    // Initial implementation
    Type1 = 1, // 4.25
    // Added MemoryMappedBulkData and ContainerHeader
    Type2 = 2, // 4.25+, 4.26 - 4.27
    // Added shader code and derived data
    Type3 = 3, // 5.0, 5.1
    // Added PackageResource
    Type4 = 4, // 5.2
}