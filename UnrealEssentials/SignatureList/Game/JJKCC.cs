﻿using UnrealEssentials.SignatureList.Engine;

namespace UnrealEssentials.SignatureList.Game;

[Signature(VersionIdentifier = "Jujutsu Kaisen CC.exe")]
public class JJKCC : UE_5_1
{
    public JJKCC() : base()
    {
        Signatures.GetPakSigningKeys = "E8 ?? ?? ?? ?? 48 8B F8 44 39 70 ??";
        Signatures.ReadBlocks = "4C 8B DC 49 89 4B ?? 53 57 41 54 41 55";
        Signatures.FileExists = "48 89 74 24 ?? 57 48 83 EC 30 45 33 C9 45 33 C0 48 8B FA 48 8B F1 E8 ?? ?? ?? ?? 84 C0 74 ?? B0 01 48 8B 74 24 ?? 48 83 C4 30 5F C3 48 89 5C 24 ?? 48 8D 4C 24 ?? 48 8B D7 48 89 6C 24 ?? 32 DB E8 ?? ?? ?? ?? 48 8D 54 24 ?? 48 8B CE E8 ?? ?? ?? ?? 48 8B 4C 24 ?? 0F B6 E8 48 85 C9 74 ?? E8 ?? ?? ?? ?? 40 84 ED 48 8B 6C 24 ?? 74 ?? 48 8B 4E ?? 48 8B D7 48 8B 01 FF 50 ??";
    }
}