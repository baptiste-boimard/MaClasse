using System.Text.Json.Serialization;

public class McpAnalysisRequest
{
  [JsonPropertyName("url")]
  public string? Url { get; set; }

  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;

  [JsonPropertyName("fileType")]
  public string FileType { get; set; } = string.Empty;

  [JsonPropertyName("base64Content")]
  public string? Base64Content { get; set; }
}