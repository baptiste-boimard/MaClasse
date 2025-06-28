using System.Globalization;
using Microsoft.AspNetCore.Components.Authorization;
using MaClasse.Client.Components;
using MaClasse.Client.Services;
using MaClasse.Client.States;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using Radzen;

var builder = WebApplication.CreateBuilder(args);
//
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys")) // Indique d'utiliser le dossier mappé
    .SetApplicationName("MaClasseSharedProd");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ServiceLogout>();
builder.Services.AddScoped<RefreshService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<UserState>();
builder.Services.AddScoped<SchedulerState>();
builder.Services.AddScoped<ViewDashboardState>();
builder.Services.AddScoped<LessonState>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//* Pour des erreurs plus détaillée
builder.Services.Configure<CircuitOptions>(options =>
{
    options.DetailedErrors = true;
});

//* Ajout des services de Radzen Blazor
builder.Services.AddRadzenComponents();

//* Ajout des services MudBlazor
builder.Services.AddMudServices();

//* Ajout de l'authentification
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

//* Gestion des cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


//* Ajoute l'authentification par cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/"; // Page de connexion
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Sécurité renforcée
    });

builder.Services.AddHttpClient();

builder.Services.AddAuthorizationCore();

//* Configuration des services Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAntiforgery();

var app = builder.Build();

app.UseCors("AllowAll"); // Applique la politique CORS dans l'app

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("fr-FR"),
    SupportedCultures = new [] { new CultureInfo("fr-FR") },
    SupportedUICultures =  new [] { new CultureInfo("fr-FR") },
});

app.UseDeveloperExceptionPage();

app.UseWebSockets();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapStaticAssets();

// app.UseStatusCodePagesWithReExecute("/NotFound", "?statusCode={0}");
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        // Redirection côté client via JavaScript
        context.HttpContext.Response.Redirect("/NotFound");
    }
});

app.Run();
