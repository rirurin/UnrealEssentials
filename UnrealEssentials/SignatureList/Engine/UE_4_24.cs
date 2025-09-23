namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE4+Release-4.24")]
public class UE_4_24 : ISignatureList
{
    protected Signatures Signatures;

    public UE_4_24()
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