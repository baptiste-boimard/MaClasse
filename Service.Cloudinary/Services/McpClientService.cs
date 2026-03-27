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
    // Construction du message JSON-RPC pour le serveur MCP
    var mcpMessage = new
      {
        jsonrpc = "2.0",
        method = "tools/call",
        @params = new { name = "analyze_file", arguments = request },
        id = Guid.NewGuid().ToString()
    };

    // Envoi au projet Serveur MCP (en supposant qu'il expose un endpoint HTTP)
    var response = await _httpClient.PostAsJsonAsync(
      $"{_configuration["Url:Mcp"]}/api/mcp/create_summary", mcpMessage);
    var content = await response.Content.ReadFromJsonAsync<JsonElement>();

    // Extraction du texte du résumé
    // Le format standard MCP renvoie : result.content[0].text
    return content.GetProperty("result")
      .GetProperty("content")[0]
      .GetProperty("text")
      .GetString();
  }
}