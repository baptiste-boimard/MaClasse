using Service.MCPServer.Interfaces;
using Service.MCPServer.Services;
//TEST PIPELINE
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
builder.Services.AddHttpClient();

// Enregistrement des services nécessaires
builder.Services.AddSingleton<McpDispatcher>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<GetAllDocumentsSummaryService>();

var app = builder.Build();

// 2. Activation du routage des contrôleurs
app.MapControllers(); 

app.Run();