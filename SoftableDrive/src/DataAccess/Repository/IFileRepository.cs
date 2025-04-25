using Microsoft.AspNetCore.Mvc;
using SoftableDrive.DataAccess.Models;

namespace SoftableDrive.DataAccess.Repository;

public interface IFileRepository
{
    Task<FileModel?> FindFile(Guid id);
    Task<List<FileModel>> FindFiles();
    Task SaveFile(FileModel file);
    Task DeleteFile(Guid id);
}
