namespace UTOC.Stream.Emulator.Interfaces;
public interface IUtocEmulator
{
    public void Initialise(TocType? tocType, PakType pakType, string fileIoStoreSig, string readBlockSig, 
        bool bFileAccessLogEnabled, Action<string> addPakFolder, Action<string> removePakFolder);

    public void AddFromFolder(string modId, string folder);
}
