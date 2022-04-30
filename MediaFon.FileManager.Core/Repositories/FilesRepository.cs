

using MediaFon.FileManager.Domain.Entity;
using MediaFon.FileManager.Infrastructure.Data;


namespace MediaFon.FileManager.Core.Repositories;



public class FilesRepository : GenericRepository<FileMetaData>, IFilesRepository
{
    public FilesRepository(ApplicationDbContext _context) : base(_context)
    {
    }

    public IEnumerable<FileMetaData> GetAllFiles() => context.Files.ToList();


}

public interface IFilesRepository
{
    IEnumerable<FileMetaData> GetAllFiles();
}
