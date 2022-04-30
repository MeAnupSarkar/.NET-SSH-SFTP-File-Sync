using MediaFon.FileManager.Core.Repositories;

namespace MediaFon.FileManager.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        FilesRepository Files { get; }
        Task<int> Complete();
    }
}