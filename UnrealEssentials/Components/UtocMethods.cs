using Reloaded.Hooks.Definitions;
using UnrealEssentials.Configuration;
using UnrealEssentials.Unreal;
namespace UnrealEssentials.Components;
using static Utils;

internal class UtocMethods
{
    // State objects
    private readonly IReloadedHooks _hooks;
    private Config _config;
    private Context _context;
    
    private IHook<GetPakSigningKeys>? _getSigningKeysHook;
    private IHook<FAsyncPackage2_StartLoading0>? _startLoading0;
    private IHook<FAsyncPackage2_StartLoading1>? _startLoading1;
    private IHook<FAsyncPackage2_StartLoading2>? _startLoading2;
    private IHook<FAsyncPackage2_StartLoading3>? _startLoading3;
    private IHook<FAsyncPackage2_StartLoading4>? _startLoading4;

    public unsafe UtocMethods(IReloadedHooks hooks, Config config, Context context)
    {
        // Get dependency objects
        _hooks = hooks;
        _config = config;
        _context = context;
        // Remove utoc signing
        SigScan(_context._signatures.GetPakSigningKeys, "GetSigningKeysPtr", address =>
        {
            var funcAddress = GetGlobalAddress(address + 1);
            LogDebug($"Found GetSigningKeys at 0x{funcAddress:X}");
            _getSigningKeysHook = _hooks.CreateHook<GetPakSigningKeys>(GetPakSigningKeysImpl, (long)funcAddress).Activate();
        });
        switch (_context._signatures.StartLoadDelegate)
        {
         
            case StartLoadingDelegateType.NoArgs:
                SigScan(
                    _context._signatures.FAsyncPackage2_StartLoading, "FAsyncPackage2::StartLoading", 
                    address => _startLoading0 = _hooks.CreateHook<FAsyncPackage2_StartLoading0>(FAsyncPackage2_StartLoading0Impl, address).Activate()
                );
                break;
            case StartLoadingDelegateType.AddIoBatch:
                SigScan(
                    _context._signatures.FAsyncPackage2_StartLoading, "FAsyncPackage2::StartLoading", 
                    address => _startLoading1 = _hooks.CreateHook<FAsyncPackage2_StartLoading1>(FAsyncPackage2_StartLoading1Impl, address).Activate()
                );
                break;
            case StartLoadingDelegateType.PackageNodeArray:
                SigScan(
                    _context._signatures.FAsyncPackage2_StartLoading, "FAsyncPackage2::StartLoading", 
                    address => _startLoading2 = _hooks.CreateHook<FAsyncPackage2_StartLoading2>(FAsyncPackage2_StartLoading2Impl, address).Activate()
                );
                break;
            case StartLoadingDelegateType.AddThreadState:
                SigScan(
                    _context._signatures.FAsyncPackage2_StartLoading, "FAsyncPackage2::StartLoading", 
                    address => _startLoading3 = _hooks.CreateHook<FAsyncPackage2_StartLoading3>(FAsyncPackage2_StartLoading3Impl, address).Activate()
                );
                break;
            case StartLoadingDelegateType.DescAddInstancingContext:
                SigScan(
                    _context._signatures.FAsyncPackage2_StartLoading, "FAsyncPackage2::StartLoading", 
                    address => _startLoading4 = _hooks.CreateHook<FAsyncPackage2_StartLoading4>(FAsyncPackage2_StartLoading4Impl, address).Activate()
                );
                break;
        }
    }
    
    internal unsafe delegate Native.FPakSigningKeys* GetPakSigningKeys();
    private unsafe Native.FPakSigningKeys* GetPakSigningKeysImpl()
    {
        // Ensure it's still a dummy key
        // Hi-Fi Rush is special and overwrites it with the actual key at some point lol
        _context._signingKeys->Function = 0;
        _context._signingKeys->Size = 0;
        return _context._signingKeys;
    }
    
    internal unsafe delegate void FAsyncPackage2_StartLoading0(Native.FAsyncPackage2* Self);
    private unsafe void FAsyncPackage2_StartLoading0Impl(Native.FAsyncPackage2* Self) 
    {
        var DiskName = Self->DiskPackageName;
        var ChunkId = new Native.FIoChunkId(Self->DiskPackageId, 0, 2);
        if (!DiskName.IsNone() && _config.FileAccessLog)
        {
            Log($"StartLoading: {DiskName}");    
        }
        _startLoading0.OriginalFunction(Self);
    }
    
    internal unsafe delegate void FAsyncPackage2_StartLoading1(Native.FAsyncPackage2_UE5_0* Self, nint IoBatch);
    private unsafe void FAsyncPackage2_StartLoading1Impl(Native.FAsyncPackage2_UE5_0* Self, nint IoBatch)
    {
        var DiskName = Self->PackagePathToLoad;
        if (!DiskName.IsNone() && _config.FileAccessLog)
        {
            Log($"StartLoading: {DiskName}");    
        }
        _startLoading1.OriginalFunction(Self, IoBatch);
    }
    
    internal unsafe delegate void FAsyncPackage2_StartLoading2(Native.FAsyncPackage2_UE5_1* Self, nint IoBatch);
    private unsafe void FAsyncPackage2_StartLoading2Impl(Native.FAsyncPackage2_UE5_1* Self, nint IoBatch) 
    {
        var DiskName = Self->PackagePathToLoad;
        if (!DiskName.IsNone() && _config.FileAccessLog)
        {
            Log($"StartLoading: {DiskName}");    
        }
        _startLoading2.OriginalFunction(Self, IoBatch);
    }
    
    internal unsafe delegate void FAsyncPackage2_StartLoading3(Native.FAsyncPackage2_UE5_3* Self, nint ThreadState, nint IoBatch);
    private unsafe void FAsyncPackage2_StartLoading3Impl(Native.FAsyncPackage2_UE5_3* Self, nint ThreadState, nint IoBatch) 
    {
        var DiskName = Self->PackagePathToLoad;
        if (!DiskName.IsNone() && _config.FileAccessLog)
        {
            Log($"StartLoading: {DiskName}");    
        }
        _startLoading3.OriginalFunction(Self, ThreadState, IoBatch);
    }
    
    internal unsafe delegate void FAsyncPackage2_StartLoading4(Native.FAsyncPackage2_UE5_4* Self, nint ThreadState, nint IoBatch);
    private unsafe void FAsyncPackage2_StartLoading4Impl(Native.FAsyncPackage2_UE5_4* Self, nint ThreadState, nint IoBatch) 
    {
        var DiskName = Self->PackagePathToLoad;
        if (!DiskName.IsNone() && _config.FileAccessLog)
        {
            Log($"StartLoading: {DiskName}");    
        }
        _startLoading4.OriginalFunction(Self, ThreadState, IoBatch);
    }
}