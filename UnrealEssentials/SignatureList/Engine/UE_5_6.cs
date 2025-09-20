using UTOC.Stream.Emulator.Interfaces;

namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE5+Release-5.6")]
public class UE_5_6 : ISignatureList
{
    protected Signatures Signatures;
    public UE_5_6()
    {
        Signatures = new Signatures
        {
            TocVersion = TocType.ReplaceIoChunkHashWithIoHash,
            PakVersion = PakType.Fn64BugFix,
            StartLoadDelegate = StartLoadingDelegateType.DescAddInstancingContext,
        };
    }

    public Signatures GetSignatures()
    {
        return Signatures;
    }   
}