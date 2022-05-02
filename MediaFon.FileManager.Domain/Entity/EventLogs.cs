using MediaFon.FileManager.Domain.Common;

namespace MediaFon.FileManager.Domain.Entity;

public class EventLogs : BaseEntity
{

    public string EventName { get; set; }

    public string JobName { get; set; }

    public string JobType { get; set; }

    public int JobIntervalInMinute { get; set; }

    public DateTime JobStartedAt { get; set; }

    public DateTime? JobEndedAt { get; set; }

    public string Result { get; set; }

   
}
