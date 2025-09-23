using UnrealEssentials.SignatureList.Engine;

namespace UnrealEssentials.SignatureList.Game;

[Signature(VersionIdentifier = "Rune Factory: Guardians of Azuma")]
public class GuardiansOfAzuma : UE_5_4
{
    public GuardiansOfAzuma() : base()
    {
        Signatures.FileIoStoreOpenContainer =
            "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 20 49 8B F1 4D 8B F0";
    }
}