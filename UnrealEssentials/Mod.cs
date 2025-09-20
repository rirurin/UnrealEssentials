using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan;
using Reloaded.Mod.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnrealEssentials.Configuration;
using UnrealEssentials.Template;
using static UnrealEssentials.Unreal.Native;
using static UnrealEssentials.Utils;
using static UnrealEssentials.Unreal.UnrealMemory;
using static UnrealEssentials.Unreal.UnrealString;
using static UnrealEssentials.Unreal.UnrealArray;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;
using Reloaded.Mod.Interfaces.Internal;
using Reloaded.Memory.Sigscan.Definitions;
using UTOC.Stream.Emulator.Interfaces;
using Reloaded.Mod.Interfaces.Structs.Enums;
using UnrealEssentials.Components;
using UnrealEssentials.Interfaces;
using UnrealEssentials.Unreal;

namespace UnrealEssentials;
/// <summary>
/// Your mod logic goes here.
/// </summary>

public unsafe class Mod : ModBase, IExports // <= Do not Remove.
{
    /// <summary>
    /// Provides access to the mod loader API.
    /// </summary>
    private readonly IModLoader _modLoader;

    /// <summary>
    /// Provides access to the Reloaded.Hooks API.
    /// </summary>
    /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
    private readonly IReloadedHooks? _hooks;

    /// <summary>
    /// Provides access to the Reloaded logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Entry point into the mod, instance that created this class.
    /// </summary>
    private readonly IMod _owner;

    /// <summary>
    /// Provides access to this mod's configuration.
    /// </summary>
    private Config _configuration;

    /// <summary>
    /// The configuration of the currently executing mod.
    /// </summary>
    private readonly IModConfig _modConfig;

    private readonly Context? _context;
    private readonly PakMethods _pakMethods;
    private readonly UtocMethods? _utocMethods;

    private IUnrealEssentials _api;
    internal static IUnrealMemory Memory;

    public Mod(ModContext context)
    {
        //Debugger.Launch();
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;

        Initialise(_logger, _configuration, _modLoader);
        _context = Context.TryCreateContext(_modLoader, _modConfig);
        if (_context == null) return;
        Memory = new UnrealMemory(_context!._signatures.GMalloc, _hooks!);
        _pakMethods = new PakMethods(_hooks!, _configuration, _context!);
        if (_context!._hasUtocs)
        {
            _utocMethods = new UtocMethods(_hooks!, _context!);
        }
        UnrealName.FNamePool.Initialize(_hooks!, _context!._signatures);

        // Gather pak files from mods
        //_modLoader.OnModLoaderInitialized += ModLoaderInit;
        _modLoader.ModLoading += ModLoading;

        // Expose API
        _api = new Api(_context!.AddFolder);
        _modLoader.AddOrReplaceController(context.Owner, _api);
    }

    private void ModLoading(IModV1 mod, IModConfigV1 modConfig)
    {
        var modsPath = Path.Combine(_modLoader.GetDirectoryForModId(modConfig.ModId), "UnrealEssentials");
        if (!Directory.Exists(modsPath))
            return;

        _context!.AddFolder(modsPath);
    }

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }
    #endregion

    public Type[] GetTypes() => new[] { typeof(IUnrealEssentials) };

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}