using System.Text.Json;
using MaClasse.Shared.Models.Api;

public class HolidaysService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HolidaysService> _logger;

    public HolidaysService(HttpClient httpClient, ILogger<HolidaysService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _httpClient.Timeout = TimeSpan.FromSeconds(200);
        _httpClient.DefaultRequestVersion = new Version(1, 1);
        _httpClient.DefaultVersionPolicy  = HttpVersionPolicy.RequestVersionExact;
    }
    
    private const string ApiUrl =
        "https://data.education.gouv.fr/api/explore/v2.1/catalog/datasets/fr-en-calendrier-scolaire/records" +
        "?select=description%2C%20start_date%2C%20end_date%2C%20zones%2C%20annee_scolaire" +
        "&where=zones%3D%22Zone%20B%22" +
        "&refine=annee_scolaire%3A%222025-2026%22" +
        "&order_by=start_date" +
        "&limit=100";

    public async Task<ApiResultResponse> GetZoneBVacationsAsync(CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ApiUrl);

        try
        {
            using var response = await _httpClient
                .SendAsync(request, HttpCompletionOption.ResponseContentRead, ct);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            return await JsonSerializer.DeserializeAsync<ApiResultResponse>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                ct
            ) ?? throw new InvalidOperationException("Réponse vide de l’API");
        }
        catch (TaskCanceledException ex)
        {
            // Remplacement de Console.WriteLine
            _logger.LogError(ex, "Appel vers l'API des vacances Zone B a dépassé le délai ({Timeout}s).", _httpClient.Timeout.TotalSeconds);
            throw;
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Erreur HTTP lors de l'appel à l'API des vacances Zone B.");
            throw;
        }
        catch (JsonException e)
        {
            _logger.LogError(e, "Erreur de désérialisation JSON de la réponse de l'API des vacances Zone B.");
            throw;
        }
    }
}