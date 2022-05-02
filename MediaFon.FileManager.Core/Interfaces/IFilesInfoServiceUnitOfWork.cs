using MediaFon.FileManager.Core.Repositories;

namespace MediaFon.FileManager.Core.Interfaces
{
    public interface IFilesInfoServiceUnitOfWork : IDisposable
    {
        FilesRepository Files { get; }
        DirectoryRepository Directories { get; }

        EventLogsRepository EventLogs{ get; }

        Task<int> Complete();
    }
}