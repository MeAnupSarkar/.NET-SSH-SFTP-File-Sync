using MediaFon.FileManager.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace MediaFon.FileManager.Domain.Entity;

public class File : BaseEntity
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string DirectoryName { get; set; }

   
    public string RemotePath { get; set; }

    public string LocalPath { get; set; }


    [MaxLength(10)]
    public string? Extention { get; set; }


    public DateTime? LastAccessTime { get; set; }

    public DateTime? LastWriteTime { get; set; }


    public DateTime? LastAccessTimeUtc { get; set; }


    public DateTime? LastWriteTimeUtc { get; set; }

    public bool HasAccessPermission { get; set; }

    public long Size { get; set; }


}
