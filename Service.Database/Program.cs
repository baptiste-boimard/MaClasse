using Microsoft.AspNetCore.DataProtection;
using Service.Database.Database;
using Service.Database.Interfaces;
using Service.Database.Repositories;
using Service.Database.Services;

var builder = WebApplication.CreateBuilder(args);

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

namespace Service.Database
{
    public partial class Program { }
}
