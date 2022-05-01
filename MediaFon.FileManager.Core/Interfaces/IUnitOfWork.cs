using MediaFon.FileManager.Core.Repositories;

namespace MediaFon.FileManager.Core.Interfaces
{
    public interface IFilesInfoServiceUnitOfWork : IDisposable
    {
        FilesRepository Files { get; }
        Task<int> Complete();
    }
}