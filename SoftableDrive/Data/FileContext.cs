using Microsoft.EntityFrameworkCore;
using SoftableDrive.Models;

namespace SoftableDrive.Data;

public class FileContext : DbContext
{
    public DbSet<FileModel> Files { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=files.sqlite");
        base.OnConfiguring(optionsBuilder);
    }
}