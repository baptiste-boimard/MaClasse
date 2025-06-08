using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Lesson;

namespace Service.Database.Services;

public class VerifyDeleteService
{
  private readonly HttpClient _httpClient;
  private readonly IConfiguration _configuration;
  private readonly UserService _userService;

  public VerifyDeleteService(
    HttpClient httpClient,
    IConfiguration configuration,
    UserService userService)
  {
    _httpClient = httpClient;
    _configuration = configuration;
    _userService = userService;
  }

  public async Task<List<Document>> VerifyDeleteFiles(RequestLesson request)
  {
    //* Vérifie que les documents ne sont pas utilisés dans d'autres collections
    List<Document> documentsToDelete = new List<Document>();


    foreach (var document in request.Lesson.Documents)
    {

      var documentRequest = new RequestLesson
      {
        Document= document,
        IdSession = request.IdSession
      };
      
      //* Recherche si le document existe dans un autre Lesson
      var response = await _httpClient.PostAsJsonAsync(
        $"{_configuration["Url:ApiGateway"]}/api/database/get-lessons-by-idDocument", documentRequest);

      if (response.IsSuccessStatusCode)
      {
        var resultDict = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        if (resultDict.Count == 1)
        {
            documentsToDelete.Add(document);
        }
      }
    }

    return documentsToDelete;
  }
}