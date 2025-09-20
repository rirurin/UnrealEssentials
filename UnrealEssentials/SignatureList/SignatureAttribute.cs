namespace UnrealEssentials.SignatureList;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SignatureAttribute : Attribute
{
    public string VersionIdentifier { get; set; }
}