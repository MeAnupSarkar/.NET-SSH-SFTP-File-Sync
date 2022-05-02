namespace MediaFon.FileManager.Domain.Common;

public class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }


    public string? CreatedBy { get; set; }

}

