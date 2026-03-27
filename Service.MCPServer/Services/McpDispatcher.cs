using System.Text.Json;
using Service.MCPServer.Interfaces;
using Service.MCPServer.Services;

namespace Service.MCPServer.Services;

public class McpDispatcher
{
  private readonly IServiceProvider _serviceProvider;

  public McpDispatcher(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task<object> Dispatch(string method, JsonElement parameters)
  {
    switch (method)
    {
      case "initialize":
        return new { protocolVersion = "2024-11-05", capabilities = new { } };

      case "tools/list":
        return new { tools = new[] { ToolDefinitions.AnalyzeFile } };

      case "tools/call":
        var toolName = parameters.GetProperty("name").GetString();
        if (toolName == "analyze_file")
        {
          var argsJson = parameters.GetProperty("arguments");
          var args = JsonSerializer.Deserialize<McpAnalysisRequest>(argsJson.GetRawText());

          using var scope = _serviceProvider.CreateScope();
          var openAi = scope.ServiceProvider.GetRequiredService<IOpenAIService>();
                    
          var result = await openAi.AnalyzeWithGptAsync(args);
          return new { content = new[] { new { type = "text", text = result } } };
        }
        throw new Exception("Tool not found");

      default:
        throw new Exception("Method not found");
    }
  }
}