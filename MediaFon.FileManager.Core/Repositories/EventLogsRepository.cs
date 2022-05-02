
using MediaFon.FileManager.Domain.Entity;
using MediaFon.FileManager.Infrastructure.Data;


namespace MediaFon.FileManager.Core.Repositories;



public class EventLogsRepository : GenericRepository<EventLogs>, IEventLogsRepository
{
    public EventLogsRepository(ApplicationDbContext _context) : base(_context)
    {
    }

    public IEnumerable<EventLogs> GetAllEvents() => context.EventLogs.ToList();


}

public interface IEventLogsRepository
{
    IEnumerable<EventLogs> GetAllEvents();
}
