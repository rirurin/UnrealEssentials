using Reloaded.Hooks.Definitions;
using System.Runtime.InteropServices;
using UnrealEssentials.Interfaces;
using static UnrealEssentials.Utils;

namespace UnrealEssentials.Unreal;
public unsafe class UnrealMemory : IUnrealMemory
{
    private FMalloc** _gMalloc;
    private IReloadedHooks _hooks;

    // FMalloc Functions
    private MallocDelegate? _malloc;
    private TryMallocDelegate? _tryMalloc;
    private ReallocDelegate? _realloc;
    private TryReallocDelegate? _tryRealloc;
    private FreeDelegate? _free;
    private QuantizeSizeDelegate? _quantizeSize;
    private GetAllocationSizeDelegate? _getAllocationSize;
    private TrimDelegate? _trim;

    internal UnrealMemory(string gMallocSig, IReloadedHooks hooks)
    {
        _hooks = hooks;
        SigScan(gMallocSig, "GMallocPtr", address =>
        {
            _gMalloc = (FMalloc**)GetGlobalAddress(address + 3);
            LogDebug($"Found GMalloc at 0x{(nuint)_gMalloc:X}");
        });
    }

    private void SetupWrappers()
    {
        // We shouldn't do allocation stuff before the game's setup
        // If we actually need to at some point we'll need to make GMalloc ourselves by hooking FMemory::GCreateMalloc
        if(*_gMalloc == null)
        {
            throw new Exception("GMalloc has not been initialised yet, please report this!");
        }

        FMallocVTable* vTable = (*_gMalloc)->VTable;
        _malloc = _hooks.CreateWrapper<MallocDelegate>((long)vTable->Malloc, out _);
        _tryMalloc = _hooks.CreateWrapper<TryMallocDelegate>((long)vTable->TryMalloc, out _);
        _realloc = _hooks.CreateWrapper<ReallocDelegate>((long)vTable->Realloc, out _);
        _tryRealloc = _hooks.CreateWrapper<TryReallocDelegate>((long)vTable->TryRealloc, out _);
        _free = _hooks.CreateWrapper<FreeDelegate>((long)vTable->Free, out _);
        _quantizeSize = _hooks.CreateWrapper<QuantizeSizeDelegate>((long)vTable->QuantizeSize, out _);
        _getAllocationSize = _hooks.CreateWrapper<GetAllocationSizeDelegate>((long)vTable->GetAllocationSize, out _);
        _trim = _hooks.CreateWrapper<TrimDelegate>((long)vTable->Trim, out _);
    }

    // Wrappers for GMalloc functions
    public nuint Malloc(nuint count, uint alignment = DEFAULT_ALIGNMENT)
    {
        if (_malloc == null)
            SetupWrappers();

        return _malloc!(*_gMalloc, count, alignment);
    }

    public nuint TryMalloc(nuint count, uint alignment = DEFAULT_ALIGNMENT)
    {
        if (_tryMalloc == null)
            SetupWrappers();

        return _tryMalloc!(*_gMalloc, count, alignment);
    }

    public nuint Realloc(nuint original, nuint count, uint alignment = DEFAULT_ALIGNMENT)
    {
        if (_realloc == null)
            SetupWrappers();

        return _realloc!(*_gMalloc, original, count, alignment);
    }

    public nuint TryRealloc(nuint original, nuint count, uint alignment = DEFAULT_ALIGNMENT)
    {
        if (_tryRealloc == null)
            SetupWrappers();

        return _tryRealloc!(*_gMalloc, original, count, alignment);
    }

    public void Free(nuint original)
    {
        if (_free == null)
            SetupWrappers();

        _free!(*_gMalloc, original);
    }

    internal nuint QuantizeSize(nuint count, uint alignment)
    {
        if (_quantizeSize == null)
            SetupWrappers();

        return _quantizeSize!(*_gMalloc, count, alignment);
    }

    public bool GetAllocationSize(nuint original, out nuint size)
    {
        if (_getAllocationSize == null)
            SetupWrappers();

        size = 0;
        fixed (nuint* sizePtr = &size )
        {   
            return _getAllocationSize!(*_gMalloc, original, sizePtr);
        }
    }

    internal void Trim(bool bTrimThreadCaches)
    {
        if (_trim == null)
            SetupWrappers();

        _trim!(*_gMalloc, bTrimThreadCaches);
    }

    // Structure definitions
    internal struct FMalloc
    {
        internal FMallocVTable* VTable;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FMallocVTable
    {
        internal nuint __vec_del_dtor;
        internal nuint exec;
        internal nuint Malloc;
        internal nuint TryMalloc;
        internal nuint Realloc;
        internal nuint TryRealloc;
        internal nuint Free;
        internal nuint QuantizeSize;
        internal nuint GetAllocationSize;
        internal nuint Trim;
    }

    // Memory Delegates
    private const int DEFAULT_ALIGNMENT = 0;

    internal delegate nuint MallocDelegate(FMalloc* gMalloc, nuint Count, uint Alignment = DEFAULT_ALIGNMENT);
    internal delegate nuint TryMallocDelegate(FMalloc* gMalloc, nuint Count, uint Alignment = DEFAULT_ALIGNMENT);
    internal delegate nuint ReallocDelegate(FMalloc* gMalloc, nuint Original, nuint Count, uint Alignment = DEFAULT_ALIGNMENT);
    internal delegate nuint TryReallocDelegate(FMalloc* gMalloc, nuint Original, nuint Count, uint Alignment = DEFAULT_ALIGNMENT);
    internal delegate void FreeDelegate(FMalloc* gMalloc, nuint Original);
    internal delegate nuint QuantizeSizeDelegate(FMalloc* gMalloc, nuint Count, uint Alignment);
    internal delegate bool GetAllocationSizeDelegate(FMalloc* gMalloc, nuint Original, nuint* SizeOut);
    internal delegate void TrimDelegate(FMalloc* gMalloc, bool bTrimThreadCaches);

    // We really don't need any of these, leaving as a comment in case there's a use in the future (I doubt it)
    //private delegate void SetupTLSCachesOnCurrentThread();
    //private delegate void ClearAndDisableTLSCachesOnCurrentThread();
    //private delegate void InitializeStatsMetadata();
    //private delegate void UpdateStats();
    //private delegate void GetAllocatorStats(FGenericMemoryStats& out_Stats);
    //private delegate void DumpAllocatorStats(class FOutputDevice& Ar);
    //private delegate bool IsInternallyThreadSafe() const;
    //private delegate bool ValidateHeap();
    //private delegate const TCHAR* GetDescriptiveName();

}
