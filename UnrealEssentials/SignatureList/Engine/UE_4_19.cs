namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE4+Release-4.19")]
public class UE_4_19 : ISignatureList
{
    protected Signatures Signatures;

    public UE_4_19()
    {
        Signatures = new Signatures
        {
            PakOpenRead = "48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 40 41 0F B6 E8"
        };
    }
    public Signatures GetSignatures()
    {
        return Signatures;
    }
}