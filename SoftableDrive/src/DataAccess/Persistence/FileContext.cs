using Microsoft.EntityFrameworkCore;
using SoftableDrive.DataAccess.Models;

namespace SoftableDrive.DataAccess.Persistence;

public class FileContext : DbContext
{
    public FileContext(DbContextOptions<FileContext> options) : base(options) {}
    public DbSet<FileModel> Files { get; set; }
}