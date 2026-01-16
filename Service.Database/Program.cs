using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Service.Database.Database;
using Service.Database.Interfaces;
using Service.Database.Repositories;
using Service.Database.Services;

var builder = WebApplication.CreateBuilder(args);

//* Limitation de la taille des requêtes à 2Mo dans tous les services autres que Cloudinary
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2 * 1024 * 1024;
});

//* Active le logging console
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys")) // Indique d'utiliser le dossier mappé
    .SetApplicationName("MaClasseSharedProd");

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<HolidaysService>();

//* Ajout des Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<HolidaysService>();
builder.Services.AddScoped<BlockVacationService>();

//* Ajout de MongoDB
builder.Services.AddSingleton<MongoDbContext>();

//* Ajout des interfaces et repositories
builder.Services.AddScoped<ISchedulerRepository, SchedulerRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();

builder.Services.AddControllers();



var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

namespace Service.Database
{
    public partial class Program { }
}
