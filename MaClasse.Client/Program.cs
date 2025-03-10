using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MaClasse.Client.Components;
using MaClasse.Client.Components.Account;
using MaClasse.Client.Data;
using MaClasse.Client.Services;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Ajout des services MudBlazor
builder.Services.AddMudServices();

// Enregistrement de vos services personnalisés
builder.Services.AddTransient<ServiceHashUrl>();
builder.Services.AddScoped<ServiceAuthentication>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpContextAccessor();

// Enregistrement d'un HttpClient avec BaseAddress (pour vos appels API)
builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri("https://localhost:7261/") });

// Configuration du DbContext et d'Identity
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// Configuration de l'authentification par cookies et Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "MaClasseAuth";
    // Pour HTTPS et pour autoriser potentiellement le cross-site, utilisez SameSite.None
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
    
    // Mappage des champs de Google vers des claims .NET
    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
});

// Ajout de l'autorisation
builder.Services.AddAuthorization();

// Configuration des services Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
// Utilisation de l'AuthenticationStateProvider fourni par Identity (IdentityRevalidatingAuthenticationStateProvider)
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Autres services spécifiques à Identity (si nécessaires)
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Ajout d'endpoints supplémentaires pour Identity (si nécessaire)
app.MapAdditionalIdentityEndpoints();

app.Run();
