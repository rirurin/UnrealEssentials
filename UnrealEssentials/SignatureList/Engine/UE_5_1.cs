﻿using UTOC.Stream.Emulator.Interfaces;

namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE5+Release-5.1")]
public class UE_5_1 : ISignatureList
{
    protected Signatures Signatures;
    public UE_5_1()
    {
        Signatures = new Signatures
        {
            GetPakSigningKeys = "E8 ?? ?? ?? ?? 48 8B F8 39 70 ??",
            GetPakFolders = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 4C 89 74 24 ?? 55 48 8B EC 48 83 EC 40 48 8D 4D ??",
            GMalloc = "48 8B 1D ?? ?? ?? ?? B9 10 00 00 00",
            GetPakOrder = "48 89 5C 24 ?? 57 48 83 EC 40 48 8B D9 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 83 78 ?? 00",
            PakOpenRead = "4C 8B DC 55 53 41 56 49 8D 6B ?? 48 81 EC C0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 66 0F 6F 05 ?? ?? ?? ??",
            PakOpenAsyncRead = "48 89 5C 24 ?? 55 56 57 48 81 EC 90 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B F2",
            IsNonPakFilenameAllowed = "48 89 5C 24 ?? 48 89 6C 24 ?? 56 57 41 56 48 83 EC 30 48 8B F1 45 33 C0",
            FileIoStoreOpenContainer = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 20 49 8B F1 4D 8B F0",
            ReadBlocks = "48 89 4C 24 ?? 56 57 41 54 41 55 41 56 41 57 48 81 EC B8 00 00 00",
            FileExists = "48 89 74 24 ?? 57 48 83 EC 30 45 33 C9 45 33 C0 48 8B FA 48 8B F1 E8 ?? ?? ?? ?? 84 C0 74 ?? B0 01 48 8B 74 24 ?? 48 83 C4 30 5F C3 48 89 5C 24 ?? 48 8D 4C 24 ?? 48 8B D7 48 89 6C 24 ?? 32 DB E8 ?? ?? ?? ?? 48 8D 54 24 ?? 48 8B CE E8 ?? ?? ?? ?? 48 8B 4C 24 ?? 0F B6 E8 48 85 C9 74 ?? E8 ?? ?? ?? ?? 40 84 ED 48 8B 6C 24 ?? 74 ?? 48 8B 4E ?? 48 8B D7 48 8B 01 FF 50 ??",
            // TocVersion = EngineVersion.UE_5_1,
            TocVersion = TocType.PerfectHash,
            PakVersion = PakType.Fn64BugFix,
            FAsyncPackage2_StartLoading = "48 89 5C 24 ?? 55 56 57 48 8D 6C 24 ?? 48 81 EC F0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 48 8B F9 48 8B F2",
            StartLoadDelegate = StartLoadingDelegateType.PackageNodeArray,
            GFNamePool = "4C 8D 05 ?? ?? ?? ?? EB ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B 4E ??",
        };
    }

    public Signatures GetSignatures()
    {
        return Signatures;
    }
}