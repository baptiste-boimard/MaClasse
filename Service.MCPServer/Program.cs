using Service.MCPServer.Interfaces;
using Service.MCPServer.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Enregistrement des services nécessaires
builder.Services.AddControllers(); // INDISPENSABLE pour utiliser les Controllers
builder.Services.AddSingleton<McpDispatcher>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();

var app = builder.Build();

// 2. Activation du routage des contrôleurs
app.MapControllers(); 

app.Run();