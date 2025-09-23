namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE4+Release-4.20")]
public class UE_4_20 : ISignatureList
{
    protected Signatures Signatures;

    public UE_4_20()
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