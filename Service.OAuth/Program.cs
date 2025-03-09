using System.Security.Claims;
using System.Text;
using MaClasse.Shared;
using MaClasse.Shared.Models;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.OAuth.Database;
using Service.OAuth.Service;

var builder = WebApplication.CreateBuilder(args);

// Configuration JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddScoped<JwtService>();
builder.Services.AddTransient<ServiceHashUrl>();

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:PostgresDBContext"]));

//* Configurer Identity (sans AddEntityFrameworkStores)
builder.Services.AddIdentity<UserProfile, IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
    })
    .AddEntityFrameworkStores<PostgresDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
    {
        //* Utilisez le cookie comme schéma par défaut pour l'authentification
        // options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        // options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //* Le schéma par défaut pour le challenge est celui de Google
        // options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    // .AddCookie(options =>
    // {
    //     options.Cookie.SameSite = SameSiteMode.None;
    //     options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    // })
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
        // googleOptions.Scope.Add("profile");
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
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

// Ajout des endpoints pour Blazor Server
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

