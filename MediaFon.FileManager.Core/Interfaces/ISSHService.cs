using MediaFon.FileManager.Domain.Entity;

namespace MediaFon.FileManager.Core.Interfaces
{
    public interface ISSHService
    {
       
        Task<EventLogs> InitRemoteSFTPSyncWithLocal();

        bool CheckIfConnected();
        bool Disconnect();


    }
}