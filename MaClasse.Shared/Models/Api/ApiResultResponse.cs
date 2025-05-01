using System.Text.Json.Serialization;

namespace MaClasse.Shared.Models.Api;

public class ApiResultResponse
{
    [JsonPropertyName("results")]
    public List<VacationRecord> Results { get; set; }
}