using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MaClasse.Shared.Models.Files;

namespace Service.Database.Services;

public class DatabaseService
{
  private readonly HttpClient _httpClient;
  private readonly CloudinaryDotNet.Cloudinary _cloudinary;
  private readonly IConfiguration _configuration;

  public DatabaseService(
    HttpClient httpClient,
    CloudinaryDotNet.Cloudinary cloudinary,
    IConfiguration configuration)
  {
    _httpClient = httpClient;
    _cloudinary = cloudinary;
    _configuration = configuration;
  }

  public async Task<Document> AddDocumentToDatabase(ImageUploadResult upload, string idUser )
  {
    var newFileRequestToDatabase = new FileRequestToDatabase
    {
      IdUser = idUser,
      Document = new Document
      {
        IdCloudinary = upload.PublicId,
        Name = upload.OriginalFilename,
        Url = upload.SecureUrl.ToString(),
        ThumbnailUrl = _cloudinary.Api.UrlImgUp
          .Transform(new Transformation().Width(150).Height(150).Crop("thumb").Gravity("auto"))
          .BuildUrl($"{upload.PublicId}.{upload.Format}"),
        Format = upload.Format,
        CreatedAt = upload.CreatedAt      
      },
      
    };
    
    var response = await _httpClient.PostAsJsonAsync(
      $"{_configuration["Url:ApiGateway"]}/api/database/add-document", newFileRequestToDatabase);

    if (response.IsSuccessStatusCode)
    {
      var newDocument = await response.Content.ReadFromJsonAsync<Document>();
      return newDocument;
    }

    return null;

  }
  
  
  
  
  
  
}