﻿using UTOC.Stream.Emulator.Interfaces;

namespace UTOC.Stream.Emulator;

public class Api : IUtocEmulator
{
    private InitialiseDelegate _initialise;
    private Action<string, string> _addFromFolder;

    internal Api(InitialiseDelegate initialise, Action<string, string> addFromFolder)
    {
        _initialise = initialise;
        _addFromFolder = addFromFolder;
    }

    public void AddFromFolder(string modId, string folder)
    {
        _addFromFolder(modId, folder);
    }

    public void Initialise(TocType? tocType, PakType pakType, string fileIoStoreSig, string readBlockSig, bool bFileAccessLogEnabled, Action<string> addPakFolder, Action<string> removePakFolder)
    {
        _initialise(tocType, pakType, fileIoStoreSig, readBlockSig, bFileAccessLogEnabled, addPakFolder, removePakFolder);
    }

    internal delegate void InitialiseDelegate(TocType? tocType, PakType pakType, string fileIoStoreSig, string readBlockSig, bool bFileAccessLogEnabled, Action<string> addPakFolder, Action<string> removePakFolder);

    public void CallGetTocFilenames(string tocPath, int version)
    {
        var chunkIds = (nint)0;
        var names = (nint)0;
        RustApi.GetTocFilenamesEx(tocPath, version, ref chunkIds, ref names);
    }
}
