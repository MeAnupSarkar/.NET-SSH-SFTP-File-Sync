

using MediaFon.FileManager.Domain.Entity;
using MediaFon.FileManager.Infrastructure.Data;


namespace MediaFon.FileManager.Core.Repositories;



public class FilesRepository : GenericRepository<Domain.Entity.File>, IFilesRepository
{
    public FilesRepository(ApplicationDbContext _context) : base(_context)
    {
    }

    public IEnumerable<Domain.Entity.File> GetAllFiles() => context.Files.ToList();


}

public interface IFilesRepository
{
    IEnumerable<Domain.Entity.File> GetAllFiles();
}
