using MediaFon.FileManager.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace MediaFon.FileManager.Domain.Entity;

public class FileMetaData : BaseEntity
{
    [Required]
    public string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public string MimeType { get; set; }

    [Required]
    [MaxLength(10)]
    public string Extention { get; set; }

    public string Location { get; set; }

    public int Size { get; set; }


}
