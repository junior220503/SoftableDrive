using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftableDrive.Data;
using SoftableDrive.Models;

namespace SoftableDrive.Controller;

[ApiController]
[Route("files")]

public class FileController : ControllerBase
{
    private readonly FileContext Context;

    public FileController(FileContext context)
    {
        Context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllFiles()
    {
        var files = await Context.Files.ToListAsync();
        return Ok(files);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var file = await Context.Files.FirstOrDefaultAsync(f => f.Id == id);
        if (file == null) return NotFound();
            
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadDir);
            
        var filePath = Path.Combine(uploadDir, file.Name);
        if (!System.IO.File.Exists(filePath))
            return NotFound("file not found.");
        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", file.Name);
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadFile(IFormFile formFile)
    {
        if (formFile.Length <= 0)
            return BadRequest("invalid file.");
            
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadDir);
            
        var filePath = Path.Combine(uploadDir, formFile.FileName);
        await using (var stream = new FileStream(filePath, FileMode.Create))
            await formFile.CopyToAsync(stream);
            
        var file = new FileModel(formFile.FileName, DateTimeOffset.UtcNow, formFile.Length);
        await Context.AddAsync(file);
        await Context.SaveChangesAsync();
            
        return Ok(file);
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteFile(Guid id)
    {
        var file = await Context.Files.FirstOrDefaultAsync(f => f.Id == id);
        if (file == null) return NotFound();
            
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadDir);
        var filePath = Path.Combine(uploadDir, file.Name);
        System.IO.File.Delete(filePath);
            
        Context.Files.Remove(file);
        await Context.SaveChangesAsync();
        return Ok(file);
    }
}