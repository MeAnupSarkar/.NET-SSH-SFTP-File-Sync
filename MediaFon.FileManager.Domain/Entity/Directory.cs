using MediaFon.FileManager.Domain.Common;

namespace MediaFon.FileManager.Domain.Entity;

public class Directory : BaseEntity
{
    public Directory()
    {
    }

    public Directory(string name, DateTime lastAccessTime, DateTime lastWriteTime, DateTime lastAccessTimeUtc, DateTime lastWriteTimeUtc, bool hasAccessPermission, long size)
    {
        Name = name;
        LastAccessTime = lastAccessTime;
        LastWriteTime = lastWriteTime;
        LastAccessTimeUtc = lastAccessTimeUtc;
        LastWriteTimeUtc = lastWriteTimeUtc;
        HasAccessPermission = hasAccessPermission;
        Size = size;
    }

    public string Name { get;  set; }

    public string RemotePath { get; set; }

    public string LocalPath { get; set; }

    public DateTime? LastAccessTime { get; set; }

    public DateTime? LastWriteTime { get; set; }

    public DateTime? LastAccessTimeUtc { get; set; }

    public DateTime? LastWriteTimeUtc { get; set; }

    public bool HasAccessPermission { get; set; }

    public long Size   { get; set; }
 
}
