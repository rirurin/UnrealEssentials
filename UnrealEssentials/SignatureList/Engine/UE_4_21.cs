﻿namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE4+Release-4.21")]
public class UE_4_21 : ISignatureList
{
    protected Signatures Signatures;

    public UE_4_21()
    {
        Signatures = new Signatures
        {
            PakOpenRead = "48 89 5C 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ?? 48 81 EC B0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 66 0F 6F 05 ?? ?? ?? ?? 48 8D 59 ??"
        };
    }
    public Signatures GetSignatures()
    {
        return Signatures;
    }
}