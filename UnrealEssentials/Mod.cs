using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan.Definitions;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using UnrealEssentials.Components;
using UnrealEssentials.Configuration;
using UnrealEssentials.Interfaces;
using UnrealEssentials.SignatureList;
using UnrealEssentials.Template;
using UnrealEssentials.Unreal;
using UTOC.Stream.Emulator.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static UnrealEssentials.Unreal.Native;
using static UnrealEssentials.Unreal.UnrealArray;
using static UnrealEssentials.Unreal.UnrealString;
using static UnrealEssentials.Utils;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;

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

        if (!Context.TryCreateContext(_modLoader, _modConfig, out _context))
            return;
        Memory = new UnrealMemory(_context!._signatures.GMalloc, _hooks, 
            _context!._signatures.AllowExecuteCommands, _context!._signatures.CommandExecutorType);
        _pakMethods = new PakMethods(_hooks!, _configuration, _context!);
        if (_context!._hasUtocs)
        {
            _utocMethods = new UtocMethods(_hooks!, _configuration, _context!);
        }
        UnrealName.FNamePool.Initialize(_hooks!, _context!._signatures);

        // Gather pak files from mods
        //_modLoader.OnModLoaderInitialized += ModLoaderInit;
        _modLoader.ModLoading += ModLoading;

        // Expose API
        _api = new Api(_context!.AddFolder, _context!.AddFolderWithVirtualMount, _context!.AddFileWithVirtualMount);
        _modLoader.AddOrReplaceController(context.Owner, _api);
    }

    private void ModLoading(IModV1 mod, IModConfigV1 modConfig)
    {
        var modRootPath = _modLoader.GetDirectoryForModId(modConfig.ModId);
        _context!.LoadUEMounts(modRootPath, Path.Combine(modRootPath, "UEMounts.yaml"));
        var modsPath = Path.Combine(modRootPath, "UnrealEssentials");
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