using Microsoft.EntityFrameworkCore;
using SoftableDrive.Domain.Models;

namespace SoftableDrive.Infrastructure.Repositories.Configuration;

public class FileContext : DbContext
{
    public FileContext(DbContextOptions<FileContext> options)
        : base(options) { }

    public DbSet<FileModel> Files { get; set; }
}
