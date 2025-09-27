using UTOC.Stream.Emulator.Interfaces;

namespace UnrealEssentials.SignatureList.Engine;

[Signature(VersionIdentifier = "++UE5+Release-5.5")]
public class UE_5_5 : ISignatureList
{
    protected Signatures Signatures;
    public UE_5_5()
    {
        Signatures = new Signatures
        {
            // TocVersion = EngineVersion.UE_5_5,
            PakVersion = PakType.Fn64BugFix,
            StartLoadDelegate = StartLoadingDelegateType.DescAddInstancingContext,
            AllowExecuteCommands = true,
            CommandExecutorType = ObjectCommandExecutorType.AddRuntime,
        };
    }

    public Signatures GetSignatures()
    {
        return Signatures;
    }   
}