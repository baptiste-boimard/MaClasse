using CloudinaryDotNet;
using Microsoft.AspNetCore.DataProtection;
using Service.Cloudinary.Interfaces;
using Service.Cloudinary.Repositories;
using Service.Database.Services;

var builder = WebApplication.CreateBuilder(args);

//* Active le logging console
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .SetApplicationName("MaClasseSharedProd");

//* Ajout des différents service
builder.Services.AddScoped<UserCloudService>();
builder.Services.AddScoped<SlugifyService>();
builder.Services.AddScoped<VerifyDeleteService>();

builder.Services.AddScoped<ICloudRepository, CloudRepository>();

builder.Services.AddHttpClient();


//* Connection a Cloudinary
builder.Services.AddSingleton(x =>
{
    var account = new Account(
        builder.Configuration["Cloudinary:CloudName"],
        builder.Configuration["Cloudinary:ApiKey"],
        builder.Configuration["Cloudinary:ApiSecret"]);

    return new Cloudinary(account);
});

//* Gestion des cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();

namespace Service.Cloudinary
{
    public partial class Program { }
}