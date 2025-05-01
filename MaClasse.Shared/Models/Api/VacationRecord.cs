using System.Text.Json.Serialization;

namespace MaClasse.Shared.Models.Api;

public class VacationRecord
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("zones")]
    public string Zone { get; set; }

    [JsonPropertyName("annee_scolaire")]
    public string AnneeScolaire { get; set; }
}