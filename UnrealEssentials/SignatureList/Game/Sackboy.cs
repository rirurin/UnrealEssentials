using UnrealEssentials.SignatureList.Engine;

namespace UnrealEssentials.SignatureList.Game;

[Signature(VersionIdentifier = "Sackboy-Win64-Shipping.exe")]
public class Sackboy: UE_4_25
{
    public Sackboy() : base()
    {
           Signatures.GetPakSigningKeys = "E8 ?? ?? ?? ?? 48 8B D8 39 78 ??";
           Signatures.GetPakFolders = "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 4C 89 74 24 ?? 55 48 8B EC 48 83 EC 40 48 8D 4D ??";
           Signatures.GMalloc = "48 89 05 ?? ?? ?? ?? E8 ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 74 ??";
           Signatures.GetPakOrder = "48 89 5C 24 ?? 57 48 83 EC 40 48 8B D9 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 83 78 08 00";
           Signatures.PakOpenRead ="48 89 5C 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ?? 48 81 EC B0 00 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 66 0F 6F 05 ?? ?? ?? ?? 48 8D 59 ??";
           Signatures.PakOpenAsyncRead = "40 55 53 56 57 41 54 41 55 48 8D 6C 24 ?? 48 81 EC A8 00 00 00";
           Signatures.IsNonPakFilenameAllowed = "48 89 5C 24 ?? 48 89 6C 24 ?? 56 57 41 56 48 83 EC 30 48 8B F1 45 33 C0";
    }      
}