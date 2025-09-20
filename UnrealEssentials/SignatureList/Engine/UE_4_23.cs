namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE4+Release-4.23")]
public class UE_4_23 : ISignatureList
{
    protected Signatures Signatures;

    public UE_4_23()
    {
        Signatures = new Signatures
        {
        };
    }
    public Signatures GetSignatures()
    {
        return Signatures;
    }
}