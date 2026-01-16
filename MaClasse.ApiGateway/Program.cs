using Microsoft.AspNetCore.DataProtection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//* Configuration du Kernel pour ne laisser entrer que des requêtes de 2Mo max
builder.WebHost.ConfigureKestrel(options =>
    {
    // Limite globale à 2 Mo pour protéger l'entrée du réseau
    options.Limits.MaxRequestBodySize = 8* 1024 * 1024; 
    });

//* Active le logging console
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys")) // Indique d'utiliser le dossier mappé
    .SetApplicationName("MaClasseSharedProd");

builder.Services.AddHttpClient();

//* Charger la configuration d'Ocelot depuis le fichier ocelot.json
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true);

//* Enregistrer les services d'Ocelot
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//* Utiliser Ocelot
await app.UseOcelot();

app.Run();
