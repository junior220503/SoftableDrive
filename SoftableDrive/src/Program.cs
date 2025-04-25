using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using SoftableDrive.Controllers;
using SoftableDrive.DataAccess.Persistence;
using SoftableDrive.DataAccess.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FileContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddControllers();

builder.Services.AddScoped<IFileRepository, FileRepository>();

var services = builder.Services;
var configuration = builder.Configuration;

builder.Configuration.AddUserSecrets<Program>();

services.AddSingleton<IAmazonS3>(provider =>
{
    var accessKey =
        configuration["AWS:AccessKey"] ?? Environment.GetEnvironmentVariable("S3_ACCESS_KEY");
    var secretKey =
        configuration["AWS:SecretKey"] ?? Environment.GetEnvironmentVariable("S3_SECRET_KEY");
    var endpoint = RegionEndpoint.USEast2;

    var config = new AmazonS3Config { RegionEndpoint = endpoint };

    return accessKey == null || secretKey == null
        ? new AmazonS3Client(config)
        : new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), config);
});

services.AddCors(options =>
{
    options.AddPolicy(
        "AllowSpecificOrigins",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000", "https://api.file-drive.softable.com.br")
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.UseHttpsRedirection();
app.Run();
