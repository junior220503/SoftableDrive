namespace SoftableDrive.Domain.Models;

public class FileModel(string name, DateTimeOffset uploadTime, long size)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public DateTimeOffset UploadTime { get; set; } = uploadTime;
    public long Size { get; set; } = size;
}
