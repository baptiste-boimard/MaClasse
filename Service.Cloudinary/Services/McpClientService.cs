using System.Text.Json;

public class McpClientService 
{
  private readonly HttpClient _httpClient;
  private readonly IConfiguration _configuration;

  public McpClientService(HttpClient httpClient, IConfiguration configuration)
  {
    _httpClient = httpClient;
    _configuration = configuration;
  }

  public async Task<string?> AnalyzeFileAsync(McpAnalysisRequest request)
  {
    if (request is null)
    {
      return null;
    }

    // Construction du message JSON-RPC pour le serveur MCP
    var mcpMessage = new
    {
      jsonrpc = "2.0",
      method = "tools/call",
      @params = new { name = "analyze_file", arguments = request },
      id = Guid.NewGuid().ToString()
    };

    // Envoi au projet Serveur MCP (en supposant qu'il expose un endpoint HTTP)
    try
    {
      var response = await _httpClient.PostAsJsonAsync(
        $"{_configuration["Url:ApiGateway"]}/api/mcp/create_summary", mcpMessage);

      if (!response.IsSuccessStatusCode)
      {
        return null;
      }

      var content = await response.Content.ReadFromJsonAsync<JsonElement>();
      if (content.ValueKind != JsonValueKind.Object)
      {
        return null;
      }

      // Le format standard MCP renvoie : result.content[0].text
      if (!content.TryGetProperty("result", out var result))
      {
        return null;
      }

      if (!result.TryGetProperty("content", out var resultContent) ||
          resultContent.ValueKind != JsonValueKind.Array ||
          resultContent.GetArrayLength() == 0)
      {
        return null;
      }

      var firstItem = resultContent[0];
      if (!firstItem.TryGetProperty("text", out var text))
      {
        return null;
      }

      return text.GetString();
    }
    catch
    {
      return null;
    }
  }
}
