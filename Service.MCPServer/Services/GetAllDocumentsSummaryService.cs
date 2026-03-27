using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Lesson;

namespace Service.MCPServer.Services;

public class GetAllDocumentsSummaryService
{
  private readonly HttpClient _httpClient;
  private readonly IConfiguration _configuration;

  public GetAllDocumentsSummaryService(
    HttpClient httpClient,
    IConfiguration configuration)
  {
    _httpClient = httpClient;
    _configuration = configuration;
  }

  public async Task<List<Document>> GetAllDocuments(RequestDocuments request)
  {
    var response = await _httpClient.PostAsJsonAsync(
      $"{_configuration["Url:ApiGateway"]}/api/database/get_all_documents", request);
    
    if (!response.IsSuccessStatusCode)
    {
      return new List<Document>();
    }
    
    var allDocuments = await response.Content.ReadFromJsonAsync<List<Document>>();

    if (allDocuments is null)
    {
      return new List<Document>();
    }
    
    return allDocuments;
  }
}