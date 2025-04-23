using Microsoft.EntityFrameworkCore;
using SoftableDrive.Models;

namespace SoftableDrive.Data;

public class FileContext : DbContext
{
    public FileContext(DbContextOptions<FileContext> options) : base(options) {}
    public DbSet<FileModel> Files { get; set; }
}