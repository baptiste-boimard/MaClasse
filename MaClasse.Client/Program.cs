using Microsoft.AspNetCore.Components.Authorization;
using MaClasse.Client.Components;
using MaClasse.Client.Services;
using MaClasse.Client.States;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server;
using MudBlazor.Services;
using Service.OAuth.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserState>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//* Pour des erreurs plus détaillée
// builder.Services.Configure<CircuitOptions>(options =>
// {
//     options.DetailedErrors = true;
// });

//* Ajout des services MudBlazor
builder.Services.AddMudServices();

//* Service de Hash pour la gestion des erreurs lors du login ou inscription
builder.Services.AddTransient<ServiceHashUrl>();

//* Service de stockage du token
builder.Services.AddScoped<ServiceAuthentication>();

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
builder.Services.AddScoped(_ => new HttpClient());


builder.Services.AddAuthorizationCore();

//* Configuration des services Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.UseCors("AllowAll"); // Applique la politique CORS dans l'app

app.UseDeveloperExceptionPage();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapStaticAssets();

app.Run();
