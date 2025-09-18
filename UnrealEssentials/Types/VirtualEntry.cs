using YamlDotNet.Serialization;

public class VirtualEntry
{
    [YamlMember(Alias = "virtual_path")]
    public required string VirtualPath { get; set; }

    [YamlMember(Alias = "os_path")]
    public required string OSPath { get; set; }
}