using System.Text;
using System.Text.Json;
using MaClasse.Shared.Models.Files;
using Microsoft.AspNetCore.Components.Forms;

namespace MaClasse.Client.States;

public class FileState
{
  private readonly UserState _userState;
  private readonly HttpClient _httpClient;
  private readonly IConfiguration _configuration;

  public FileState(
    UserState userState,
    HttpClient httpClient,
    IConfiguration configuration)
  {
    _userState = userState;
    _httpClient = httpClient;
    _configuration = configuration;
  }
  
  public List<IBrowserFile> Files { get; set; }

  public async Task<List<IBrowserFile>> UploadFile(IBrowserFile file)
  {
    var content = new MultipartFormDataContent();
    
    // Ajout du fichier 
    var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
    content.Add(new StreamContent(stream), "file", file.Name);
    
    // Ajouter les métadonnées (JSON sous forme de StringContent)
    var request = new FileRequest { IdSession = _userState.IdSession };
    var json = JsonSerializer.Serialize(request);
    content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "filerequest");
    
    var response =
      await _httpClient.PostAsync(
        $"{_configuration["Url:ApiGateway"]}/api/cloud/add-file", content);

    if (response.IsSuccessStatusCode)
    {
            
    }
    return null;
  }
}