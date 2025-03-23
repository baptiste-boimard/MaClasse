using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Service.OAuth.Database;
using Service.OAuth.Interfaces;
using Service.OAuth.Repositories;
using Service.OAuth.Service;

var builder = WebApplication.CreateBuilder(args);

//* Service de Hash pour la gestion des erreurs lors du login ou inscription
builder.Services.AddTransient<ServiceHashUrl>();

//* Ajout des interfaces et repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ValidateGoogleTokenService>();

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
