﻿using UTOC.Stream.Emulator.Interfaces;

namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE5+Release-5.3")]
public class UE_5_3 : ISignatureList
{
    protected Signatures Signatures;
    public UE_5_3()
    {
        Signatures = new Signatures
        {
            GetPakSigningKeys = "E8 ?? ?? ?? ?? 48 8B F8 39 70 ??",
            GetPakFolders = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 4C 89 74 24 ?? 55 48 8B EC 48 83 EC 40 48 8D 4D ??",
            GMalloc = "48 89 05 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ??",
            GetPakOrder = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 40 48 8B E9 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 1D ?? ?? ?? ??",
            PakOpenRead = "4C 8B DC 55 53 57 49 8D 6B ?? 48 81 EC C0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 66 0F 6F 05 ?? ?? ?? ??",
            PakOpenAsyncRead = "48 89 5C 24 ?? 48 89 74 24 ?? 55 57 41 56 48 8D 6C 24 ?? 48 81 EC 90 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 48 8B F2",
            IsNonPakFilenameAllowed = "48 89 5C 24 ?? 55 56 57 41 56 41 57 48 83 EC 30 48 8B E9 45 33 C0",
            FileIoStoreOpenContainer = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 20 49 8B F1 4D 8B F0",
            ReadBlocks = "4C 8B DC 49 89 4B ?? 53 57 41 54",
            FileExists = "48 89 74 24 ?? 57 48 83 EC 30 45 33 C9 45 33 C0 48 8B FA 48 8B F1 E8 ?? ?? ?? ?? 84 C0 74 ?? B0 01 48 8B 74 24 ?? 48 83 C4 30 5F C3 48 89 5C 24 ?? 48 8D 4C 24 ?? 48 8B D7 48 89 6C 24 ?? 32 DB E8 ?? ?? ?? ?? 48 8D 54 24 ?? 48 8B CE E8 ?? ?? ?? ?? 48 8B 4C 24 ?? 0F B6 E8 48 85 C9 74 ?? E8 ?? ?? ?? ?? 40 84 ED 48 8B 6C 24 ?? 74 ?? 48 8B 4E ?? 48 8B D7 48 8B 01 FF 50 ??",
            TocVersion = TocType.PerfectHash,
            PakVersion = PakType.Fn64BugFix,
            FAsyncPackage2_StartLoading = "48 89 5C 24 ?? 55 56 57 48 8D 6C 24 ?? 48 81 EC F0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 48 8B F9 49 8B F0",
            StartLoadDelegate = StartLoadingDelegateType.AddThreadState,
            GFNamePool = "4C 8D 05 ?? ?? ?? ?? EB ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 4C 8B C0 C6 05 ?? ?? ?? ?? 01 0F B7 C3",
            ChunkIdType = TocChunkIdType.Type4,
        };
    }

    public Signatures GetSignatures()
    {
        return Signatures;
    }
}