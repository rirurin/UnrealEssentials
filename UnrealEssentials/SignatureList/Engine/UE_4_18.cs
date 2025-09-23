namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE4+Release-4.18")]
public class UE_4_18 : ISignatureList
{
    protected Signatures Signatures;

    public UE_4_18()
    {
        Signatures = new Signatures
        {
            PakOpenRead = "48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 40 41 0F B6 E8 48 C7 44 24 ?? 00 00 00 00"
        };
    }
    public Signatures GetSignatures()
    {
        return Signatures;
    }
}