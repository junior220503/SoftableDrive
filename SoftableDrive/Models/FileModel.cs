namespace SoftableDrive.Models;

public class FileModel
{
    public FileModel(string name, DateTimeOffset uploadTime, long size)
    {
        Id = Guid.NewGuid();
        Name = name;
        UploadTime = uploadTime;
        Size = size;
    }
    
    public Guid Id { get; init; }
    public string Name { get; set; }
    public DateTimeOffset UploadTime { get; set; }
    public long Size { get; set; }
}