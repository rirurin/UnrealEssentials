using System.Runtime.InteropServices;
using UTOC.Stream.Emulator.Interfaces;

namespace UTOC.Stream.Emulator
{
    public static unsafe class RustApi
    {
        
        const string __DllName = "fileemu_utoc_stream_emulator";

        [DllImport(__DllName, EntryPoint = "add_from_folders", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)] // Collect assets
        internal static extern void AddFromFolders(nint modPath, nint modPathLength);
        
        [DllImport(__DllName, EntryPoint = "close_folder_threads", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        internal static extern void CloseFolderThreads();
        
        [DllImport(__DllName, EntryPoint = "set_reloaded_logger", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        internal static extern void SetReloadedLogger(delegate* unmanaged[Stdcall]<nint, nint, int, void> offset);
        
        [DllImport(__DllName, EntryPoint = "set_toc_version", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        internal static extern void SetTocVersion(EngineVersion toc);
        
        [DllImport(__DllName, EntryPoint = "setup_folder_threads", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        internal static extern void SetupFolderThreads();
    }
}
