using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Mod.Interfaces;
using UnrealEssentials.Configuration;

namespace UnrealEssentials.Components;
using static Utils;

internal class DebugComponent
{
    // State objects
    private readonly IReloadedHooks _hooks;
    private IModLoader _modLoader;
    private IModConfig _modConfig;
    private Config _configuration;
    private Context _context;

    private int DumpCount = 0;

    private IHook<ReadUcasContainerHeader>? _readUcasContainerHeader;

    public unsafe DebugComponent(IReloadedHooks hooks, IModLoader modLoader, IModConfig modConfig, Config configuration, Context context)
    {
        _hooks = hooks;
        _modLoader = modLoader;
        _modConfig = modConfig;
        _configuration = configuration;
        _context = context;
       
        /*
        SigScan(
            "48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 55 41 54 41 55 41 56 41 57 48 8D AC 24 ?? ?? ?? ?? 48 81 EC 30 02 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 ?? ?? ?? ?? 48 8B 41 ??",
            "ReadUcasContainerHeader",
            address => _readUcasContainerHeader = _hooks.CreateHook<ReadUcasContainerHeader>(ReadUcasContainerHeaderImpl, address).Activate()
            );
        */
    }
    
    [StructLayout(LayoutKind.Explicit)]
    private unsafe struct BufCore
    {
        [FieldOffset(0x0)] internal nint DataPtr;
        [FieldOffset(0x8)] internal int Size;
    }

    [StructLayout(LayoutKind.Explicit)]
    private unsafe struct LambdaParams
    {
        [FieldOffset(0x18)] internal BufCore* Data;
    }

    private unsafe delegate void ReadUcasContainerHeader(LambdaParams* lambda);

    private unsafe void ReadUcasContainerHeaderImpl(LambdaParams* lambda)
    {
        var dumpPath = Path.Combine(_modLoader.GetDirectoryForModId(_modConfig.ModId), $"Container{DumpCount}.bin");
        DumpCount++;
        var Window = new Span<byte>((byte*)lambda->Data->DataPtr, lambda->Data->Size);
        LogDebug($"Written {Window.Length} bytes into {dumpPath}");
        File.WriteAllBytes(dumpPath, Window.ToArray());
        
        _readUcasContainerHeader.OriginalFunction(lambda);
    }
}