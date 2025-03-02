using System.Security.Claims;
using MaClasse.Shared;
using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.OAuth.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:PostgresDBContext"]));

//* Configurer Identity avec le modèle d'utilisateur que vous utilisez
builder.Services.AddIdentity<UserProfile, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<PostgresDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
    {
        //* Utilisez le cookie comme schéma par défaut pour l'authentification
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //* Le schéma par défaut pour le challenge est celui de Google
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
        
        //* Mappez les champs JSON de Google vers des claims .NET
        googleOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
        googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
        
        //* Pour récupérer le profil complet, vous pouvez ajouter le scope "profile"
        googleOptions.Scope.Add("profile");
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

//* Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

