
using MediaFon.FileManager.Infrastructure.Data;
using Directory = MediaFon.FileManager.Domain.Entity.Directory;

namespace MediaFon.FileManager.Core.Repositories;



public class DirectoryRepository : GenericRepository<Directory>, IDirectoryRepository
{
    public DirectoryRepository(ApplicationDbContext _context) : base(_context)
    {
    }

    public IEnumerable<Directory> GetAllDirectories() => context.Directories.ToList();


}

public interface IDirectoryRepository
{
    IEnumerable<Directory> GetAllDirectories();
}
