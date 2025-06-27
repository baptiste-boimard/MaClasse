using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Service.OAuth.Database;
using Service.OAuth.Interfaces;
using Service.OAuth.Repositories;
using Service.OAuth.Service;
using Service.OAuth.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys")) // Indique d'utiliser le dossier mappé
    .SetApplicationName("MaClasseSharedProd");

//* Ajout des interfaces et repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IRattachmentRepository, RattachementRepository>();

//* Ajout des services
builder.Services.AddScoped<ICreateDataService, CreateDataService>();
builder.Services.AddScoped<IValidateGoogleTokenService, ValidateGoogleTokenService>();
builder.Services.AddScoped<IGenerateIdRole, GenerateIdRole>();
builder.Services.AddScoped<IUserServiceRattachment, UserServiceRattachment>();
builder.Services.AddScoped<IDeleteUserService, DeleteUserService>();


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

//* Ajout de l'authentification
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

//* Configuration du DbContext
builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:PostgresDBContext"]));

builder.Services.AddAuthorization();

builder.Services.AddControllers();

//* Ajouter les services nécessaires pour Blazor
builder.Services.AddRazorComponents();

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
