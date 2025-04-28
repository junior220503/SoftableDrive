using SoftableDrive.Domain.Models;

namespace SoftableDrive.Application.Files;

public interface IFileRepository
{
    Task<FileModel?> FindFile(Guid id);
    Task<List<FileModel>> FindFiles();
    Task SaveFile(FileModel file);
    Task DeleteFile(Guid id);
}
