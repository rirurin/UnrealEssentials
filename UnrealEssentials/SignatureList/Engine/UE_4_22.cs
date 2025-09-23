namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE4+Release-4.22")]
public class UE_4_22 : ISignatureList
{
    
    protected Signatures Signatures;

    public UE_4_22()
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