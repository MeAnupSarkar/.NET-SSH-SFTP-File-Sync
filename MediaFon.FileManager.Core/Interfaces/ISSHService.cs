using MediaFon.FileManager.Domain.Entity;

namespace MediaFon.FileManager.Core.Interfaces
{
    public interface ISSHService
    {
       
        Task  InitRemoteSFTPSyncWithLocal();

        bool CheckIfConnected();
        bool Disconnect();


    }
}