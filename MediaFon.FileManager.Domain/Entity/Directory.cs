using MediaFon.FileManager.Domain.Common;


namespace MediaFon.FileManager.Domain.Entity;

public class Directory   : BaseEntity
{
    public Directory(){}

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
