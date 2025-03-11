using System.Security.Claims;
using System.Text;
using MaClasse.Shared;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.OAuth.Database;
using Service.OAuth.Service;

var builder = WebApplication.CreateBuilder(args);

// Service personnalisé
builder.Services.AddTransient<ServiceHashUrl>();
builder.Services.AddScoped<JwtService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


// Configuration du DbContext et d'Identity
builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:PostgresDBContext"]));

builder.Services.AddIdentity<UserProfile, IdentityRole<Guid>>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
})
.AddEntityFrameworkStores<PostgresDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Configuration de l'authentification par cookies et Google
builder.Services.AddAuthentication(options =>
{
    // On utilise le schéma de cookies pour l'authentification et le challenge.
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "MaClasseAuth";
    // Pour HTTPS et autoriser potentiellement le cross-site, utilisez SameSite.None.
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
    
    // Mappage des claims de Google vers les claims .NET
    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy =>
        policy.RequireAuthenticatedUser());
});

// (Optionnel) Pour Swagger/OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
