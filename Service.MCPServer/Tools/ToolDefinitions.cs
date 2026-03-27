using System.Text.Json;
using ModelContextProtocol.Protocol; // Vérifiez le namespace exact de votre SDK

public static class ToolDefinitions
{
  public static Tool AnalyzeFile => new Tool
  {
    Name = "analyze_file",
    Description = "Analyse un document Cloudinary (Image ou PDF) et retourne un résumé.",
    // On utilise JsonSerializer pour créer le JsonElement attendu
    InputSchema = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new
    {
      type = "object",
      properties = new
      {
        url = new { type = "string", description = "URL sécurisée Cloudinary" },
        name = new { type = "string", description = "Nom du fichier" },
        fileType = new { type = "string", @enum = new[] { "image", "pdf" } }
      },
      required = new[] { "url", "name", "fileType" }
    }))
  };
}