using UTOC.Stream.Emulator.Interfaces;

namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE5+Release-5.7")]
public class UE_5_7 : ISignatureList
{
    protected Signatures Signatures;
    public UE_5_7()
    {
        Signatures = new Signatures
        {
            // TocVersion = EngineVersion.UE_5_7,
            PakVersion = PakType.Fn64BugFix,
            StartLoadDelegate = StartLoadingDelegateType.AsyncPackageInheritsRefCount,
        };
    }

    public Signatures GetSignatures()
    {
        return Signatures;
    }   
}