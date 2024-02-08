using System.Runtime.InteropServices;

namespace UTOC.Stream.Emulator.Interfaces;

public enum TocType
{
    Initial = 1, // 4.25
    DirectoryIndex = 2, // 4.25+, 4.26
    PartitionSize = 3, // 4.27
    PerfectHash = 4, // 5.0+
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

[StructLayout(LayoutKind.Sequential, Size = 0xc)]
public unsafe struct FIoChunkId : IEquatable<FIoChunkId>
{
    public byte GetByte(int idx) { fixed (FIoChunkId* self = &this) return *(byte*)((IntPtr)self + idx); }
    public string GetId()
    {
        string key_out = "0x";
        for (int i = 0; i < 0xc; i++) key_out += $"{GetByte(i):X2}";
        return key_out;
    }
    public bool Equals(FIoChunkId other)
    {
        fixed (FIoChunkId* self = &this)
        {
            if (*(ulong*)self != *(ulong*)&other) return false;
            return true;
        }
    }
}

