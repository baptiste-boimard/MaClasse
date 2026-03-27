using System.Text.Json;

namespace Service.MCPServer.Services;

public class McpWorker : BackgroundService
{
  private readonly McpDispatcher _dispatcher;

  public McpWorker(McpDispatcher dispatcher) => _dispatcher = dispatcher;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    using var reader = new StreamReader(Console.OpenStandardInput());

    while (!stoppingToken.IsCancellationRequested && !reader.EndOfStream)
    {
      var json = await reader.ReadLineAsync();
      if (string.IsNullOrEmpty(json)) continue;

      try
      {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
                
        var method = root.GetProperty("method").GetString();
        root.TryGetProperty("id", out var id);
        var parameters = root.TryGetProperty("params", out var p) ? p : default;

        var result = await _dispatcher.Dispatch(method, parameters);

        // Envoi de la réponse sur la sortie standard
        var response = new { jsonrpc = "2.0", id = id, result = result };
        Console.WriteLine(JsonSerializer.Serialize(response));
        await Console.Out.FlushAsync();
      }
      catch (Exception ex)
      {
        await Console.Error.WriteLineAsync($"MCP Error: {ex.Message}");
      }
    }
  }
}