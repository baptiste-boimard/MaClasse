using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MaClasse.Client.Components;
using MaClasse.Client.Components.Account;
using MaClasse.Client.Data;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using MudBlazor.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddTransient<ServiceHashUrl>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton(
    new HttpClient { BaseAddress = new Uri("https://localhost:7261/") });


//* Configuration du DbContext et d'Identity
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

// Service pour l'envoi d'emails (dummy)
// builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Configuration unique de l'authentification et de l'autorisation
builder.Services.AddAuthenticationCore();
// builder.Services.AddAuthentication(options =>
    // {
        // Utilisation du cookie comme schéma par défaut pour l'authentification
        // options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        // options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        // Utilisation de Google pour les défis externes
        // options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    // })
    // .AddCookie(options =>
    // {
        // Utilisez SameSiteMode.Lax ou SameSiteMode.None selon vos besoins
        // options.Cookie.SameSite = SameSiteMode.Lax;
    // });
    // .AddGoogle(options =>
    // {
    //     options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    //     options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        // Le CallbackPath par défaut est "/signin-google"
        // Vous pouvez le modifier si nécessaire, par exele :
        // options.CallbackPath = new PathString("/signin-google");
    // });

//* Ajouter l'autorisation
builder.Services.AddAuthorization();


//* Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

//* Activation de l’authentification/autorisation dans le pipeline
app.UseAuthentication();
app.UseAuthorization();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
