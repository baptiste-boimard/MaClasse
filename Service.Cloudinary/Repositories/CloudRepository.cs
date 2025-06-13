using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Service.Cloudinary.Interfaces;
using Service.Database.Services;

namespace Service.Cloudinary.Repositories;

public class CloudRepository : ICloudRepository
{
  private readonly CloudinaryDotNet.Cloudinary _cloudinary;
  private readonly SlugifyService _slugifyService;
  private ICloudRepository _cloudRepositoryImplementation;

  public CloudRepository(
    CloudinaryDotNet.Cloudinary cloudinary,
    SlugifyService slugifyService)
  {
    _cloudinary = cloudinary;
    _slugifyService = slugifyService;
  }
  
  public async Task<UploadResult> UploadFileAsync(IFormFile file, string idUser)
  {
    
    if (file == null || file.Length == 0)
      throw new ArgumentException("Fichier invalide");

    await using var memoryStream = new MemoryStream();
    await file.CopyToAsync(memoryStream);
    memoryStream.Position = 0;

    //* Traitement du nom du fichier pour enlever les caractères génants
    var nameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);
    var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
    var finalFileName = _slugifyService.SlugifyFileName($"{nameWithoutExt}{extension}");
    
    var imageLikeExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    
    if (imageLikeExtensions.Contains(extension))
    {
      var imageParams = new ImageUploadParams
      {
        File = new FileDescription(finalFileName, memoryStream),
        Folder = idUser,
        UseFilename = true,
        UniqueFilename = true,
        Overwrite = false,
        AccessMode = "public", 
        Type = "upload",
        UploadPreset = "Public"

      };

      var uploadResult = await _cloudinary.UploadAsync(imageParams);
      
      return uploadResult;

    }
    return null;
  }

  public async Task<GetResourceResult> GetFileAsyncByIdCloudinary(string idCloudinary)
  {
    var existingDocument = await _cloudinary.GetResourceAsync(new GetResourceParams(idCloudinary));

    if (existingDocument == null) return null;
    
    return existingDocument;
  }

  public Task<ImageUploadResult> UpdateFileAsync()
  {
    return _cloudRepositoryImplementation.UpdateFileAsync();
  }

  public async Task<DelResResult> DeleteFileAsync(string idCloudinary)
  {
    var deletedDocument = await _cloudinary.DeleteResourcesAsync(idCloudinary);

    if (!deletedDocument.Deleted.TryGetValue(idCloudinary, out var status)) return null;
      
    return deletedDocument;
  }

  public async Task<RenameResult> RenameFileAsync(string oldPublicId, string newPublicId)
  {
    //* Traitement du nom du fichier pour enlever les caractères génants
    var slugNewPublicId = _slugifyService.SlugifyFileName(newPublicId);

    var renameParams = new RenameParams(oldPublicId, slugNewPublicId);

    var result = await _cloudinary.RenameAsync(renameParams);

    if (result.StatusCode != System.Net.HttpStatusCode.OK) return null;
    
    return result;
  }
  
  public async Task<ImageUploadResult> GetFilesAsync()
  {
    return null;
  }
}