using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftableDrive.DataAccess.Models;
using SoftableDrive.DataAccess.Persistence;

namespace SoftableDrive.Controllers;

[ApiController]
[Route("files")]

public class FileController : ControllerBase
{
    private readonly FileContext Context;
    private readonly IAmazonS3 S3Client;

    public FileController(FileContext context, IAmazonS3 s3Client)
    {
        Context = context;
        S3Client = s3Client;
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

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = "softabledrive",
                Key = file.Id.ToString()
            };
            
            using var response = await S3Client.GetObjectAsync(request);
            var stream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            
            return File(stream, "application/octet-stream", file.Name);
        }
        catch (AmazonS3Exception e)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadFile(IFormFile formFile)
    {
        if (formFile.Length <= 0)
            return BadRequest("invalid file.");

        try
        {
            var file = new FileModel(formFile.FileName, DateTime.UtcNow, formFile.Length);

            var request = new PutObjectRequest
            {
                BucketName = "softabledrive",
                Key = file.Id.ToString(),
                InputStream = formFile.OpenReadStream(),
                ContentType = "application/octet-stream"
            };
            
            await S3Client.PutObjectAsync(request);
            
            await Context.AddAsync(file);
            await Context.SaveChangesAsync();
            
            return Ok();
        }
        catch (AmazonS3Exception e)
        {
            return StatusCode(500, $"Error uploading file: {e.Message}");
        }
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteFile(Guid id)
    {
        var file = await Context.Files.FirstOrDefaultAsync(f => f.Id == id);
        if (file == null) return NotFound();

        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = "softabledrive",
                Key = file.Id.ToString()
            };
            await S3Client.DeleteObjectAsync(request);
            
            Context.Files.Remove(file);
            await Context.SaveChangesAsync();
            
            return Ok();
        }
        catch (AmazonS3Exception e)
        {
            return StatusCode(500, $"Error deleting file: {e.Message}");
        }
    }
}