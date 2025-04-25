using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftableDrive.DataAccess.Models;
using SoftableDrive.DataAccess.Persistence;
using SoftableDrive.DataAccess.Repository;

namespace SoftableDrive.Controllers;

[ApiController]
[Route("files")]
public class FileController(IFileRepository repository, IAmazonS3 s3Client) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllFiles()
    {
        var files = await repository.FindFiles();
        return Ok(files);
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var file = await repository.FindFile(id) ?? throw new FileNotFoundException();

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = "softabledrive",
                Key = file.Id.ToString(),
            };

            using var response = await s3Client.GetObjectAsync(request);
            var stream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/octet-stream", file.Name);
        }
        catch (AmazonS3Exception)
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
                ContentType = "application/octet-stream",
            };

            await s3Client.PutObjectAsync(request);

            await repository.SaveFile(file);

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
        var file = await repository.FindFile(id) ?? throw new FileNotFoundException();

        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = "softabledrive",
                Key = id.ToString(),
            };
            await s3Client.DeleteObjectAsync(request);

            await repository.DeleteFile(id);

            return Ok();
        }
        catch (AmazonS3Exception e)
        {
            return StatusCode(500, $"Error deleting file: {e.Message}");
        }
    }
}
