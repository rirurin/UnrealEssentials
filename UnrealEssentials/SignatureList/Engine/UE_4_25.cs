﻿using UTOC.Stream.Emulator.Interfaces;

namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE4+Release-4.25")]
public class UE_4_25 : ISignatureList
{
    protected Signatures Signatures;
    public UE_4_25()
    {
        Signatures = new Signatures
        {
            GetPakSigningKeys = "E8 ?? ?? ?? ?? 48 8B D8 39 78 ??",
            GetPakFolders = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 4C 89 74 24 ?? 55 48 8B EC 48 83 EC 40 48 8D 4D ??",
            GMalloc = "48 89 05 ?? ?? ?? ?? E8 ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 74 ??",
            GetPakOrder = "48 89 5C 24 ?? 57 48 83 EC 40 48 8B D9 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 83 78 08 00",
            PakOpenRead = "48 89 5C 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ?? 48 81 EC D0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 66 0F 6F 05 ?? ?? ?? ??",
            PakOpenAsyncRead = "40 55 57 41 56 41 57 48 81 EC 98 00 00 00",
            IsNonPakFilenameAllowed = "48 8B C4 55 41 55 48 8D 68 ?? 48 81 EC 98 00 00 00",
            FileExists = "48 89 6C 24 ?? 57 48 83 EC 30 45 33 C9 45 33 C0 48 8B FA 48 8B E9 E8 ?? ?? ?? ?? 84 C0 74 ?? B0 01 48 8B 6C 24 ?? 48 83 C4 30 5F C3 33 C9 48 89 5C 24 ?? 48 89 74 24 ?? 8B D1 40 32 F6 48 89 4C 24 ?? 48 89 4C 24 ?? 48 85 FF 74 ?? 66 39 0F 74 ?? 48 C7 C3 FF FF FF FF 0F 1F 84 ?? 00 00 00 00 48 FF C3 66 39 0C ?? 75 ?? FF C3 85 DB 7E ?? 8B D3 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 8B 54 24 ?? 8B 4C 24 ?? 8D 04 ?? 89 44 24 ?? 3B C2 7E ?? 8B D1 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 4C 24 ?? 48 8B D7 4C 63 C3 4D 03 C0 E8 ?? ?? ?? ?? 48 8D 54 24 ?? 48 8B CD E8 ?? ?? ?? ?? 48 8B 4C 24 ?? 0F B6 D8 48 85 C9 74 ?? E8 ?? ?? ?? ?? 84 DB 48 8B 5C 24 ?? 74 ?? 48 8B 4D ?? 48 8B D7 48 8B 01 FF 50 ??",
            // TocVersion = EngineVersion.UE_4_25,
            TocVersion = TocType.Initial,
            PakVersion = PakType.FrozenIndex,
            AllowExecuteCommands = true,
            CommandExecutorType = ObjectCommandExecutorType.GlobalOnly,
        };
    }

    public Signatures GetSignatures()
    {
        return Signatures;
    }
}