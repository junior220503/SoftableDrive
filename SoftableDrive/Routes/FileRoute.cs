using Microsoft.EntityFrameworkCore;
using SoftableDrive.Data;
using SoftableDrive.Models;

namespace SoftableDrive.Routes;

public static class FileRoute
{
    public static void FileRoutes(this WebApplication app)
    {
        app.MapGroup("files");
        
        app.MapPost("", async (FileContext context, IFormFile formFile) =>
        {
            if (formFile.Length <= 0)
                return Results.BadRequest("invalid file.");
            
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploadDir);
            
            var filePath = Path.Combine(uploadDir, formFile.FileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
                await formFile.CopyToAsync(stream);
            
            var file = new FileModel(formFile.FileName, DateTime.Now, formFile.Length);
            await context.AddAsync(file);
            await context.SaveChangesAsync();
            
            return Results.Ok(file);
        })
        .DisableAntiforgery();
        
        app.MapGet("", async (FileContext context) =>
        {
            var files = await context.Files.ToListAsync();
            return Results.Ok(files);
        });
        
        app.MapGet("{id:guid}", async (Guid id, FileContext context) =>
        {
            var file = await context.Files.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null) return Results.NotFound();
            
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploadDir);
            
            var filePath = Path.Combine(uploadDir, file.Name);
            if (!File.Exists(filePath))
                return Results.NotFound("file not found.");
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            return Results.File(fileBytes, "application/octet-stream", file.Name);
        });
        
        app.MapDelete("{id:guid}", async (Guid id, FileContext context) =>
        {
            var file = await context.Files.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null) return Results.NotFound();
            
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploadDir);
            var filePath = Path.Combine(uploadDir, file.Name);
            File.Delete(filePath);
            
            context.Files.Remove(file);
            await context.SaveChangesAsync();
            return Results.Ok(file);
        });
    }
}