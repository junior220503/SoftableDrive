using SoftableDrive.Data;
using SoftableDrive.Models;
using SoftableDrive.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<FileContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.FileRoutes();

app.UseHttpsRedirection();
app.Run();