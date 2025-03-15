using Microsoft.AspNetCore.Components.Authorization;
using MaClasse.Client.Components;
using MaClasse.Client.Services;
using MaClasse.Shared.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

//* Ajout des services MudBlazor
builder.Services.AddMudServices();

//* Service de Hash pour la gestion des erreurs lors du login ou inscription
builder.Services.AddTransient<ServiceHashUrl>();

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
        options.Cookie.HttpOnly = false;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Sécurité renforcée
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
