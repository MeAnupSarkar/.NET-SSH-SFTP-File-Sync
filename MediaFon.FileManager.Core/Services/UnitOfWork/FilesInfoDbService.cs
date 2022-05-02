using MediaFon.FileManager.Core.Interfaces;
using MediaFon.FileManager.Core.Repositories;
using MediaFon.FileManager.Infrastructure.Data;
 

namespace MediaFon.FileManager.Core.UnitOfWork.Services
{
    public class FilesInfoDbService : IFilesInfoServiceUnitOfWork 
    {
        private readonly ApplicationDbContext _context;

        public FilesRepository Files { get; private set; }
        public DirectoryRepository Directories { get; private set; }

        public EventLogsRepository EventLogs { get; private set; }

        public FilesInfoDbService(ApplicationDbContext context)
        {
            _context = context;
            Files = new FilesRepository(_context);
            Directories = new DirectoryRepository(_context);
            EventLogs = new EventLogsRepository(_context);
        }
 

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
