using Microsoft.EntityFrameworkCore;
using SoftableDrive.DataAccess.Models;
using SoftableDrive.DataAccess.Persistence;

namespace SoftableDrive.DataAccess.Repository;

public class FileRepository(FileContext context) : IFileRepository
{
    public async Task<FileModel?> FindFile(Guid id)
    {
        return await context.Files.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<List<FileModel>> FindFiles()
    {
        return await context.Files.ToListAsync();
    }

    public async Task SaveFile(FileModel file)
    {
        await context.Files.AddAsync(file);
        await context.SaveChangesAsync();
    }

    public async Task DeleteFile(Guid id)
    {
        var file = await FindFile(id) ?? throw new KeyNotFoundException("File not found");
        context.Files.Remove(file);
        await context.SaveChangesAsync();
    }
}
