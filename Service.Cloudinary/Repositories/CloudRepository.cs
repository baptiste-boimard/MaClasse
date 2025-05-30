using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MaClasse.Shared.Models.Files;
using Service.Cloudinary.Interfaces;

namespace Service.Cloudinary.Repositories;

public class CloudRepository : ICloudRepository
{
  private readonly CloudinaryDotNet.Cloudinary _cloudinary;
  private ICloudRepository _cloudRepositoryImplementation;

  public CloudRepository(CloudinaryDotNet.Cloudinary cloudinary)
  {
    _cloudinary = cloudinary;
  }
  
  public async Task<ImageUploadResult> UploadFileAsync(IFormFile file, string idUser)
  {
    if (file == null || file.Length == 0)
      throw new ArgumentException("Fichier invalide");

    await using var memoryStream = new MemoryStream();
    await file.CopyToAsync(memoryStream);
    memoryStream.Position = 0;

    var uploadParams = new ImageUploadParams
    {
      File = new FileDescription(file.FileName, memoryStream),
      Folder = idUser,
      UseFilename = true,
      UniqueFilename = true,
      Overwrite = false
    };
    
    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
    
    return uploadResult;
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
    var renameParams = new RenameParams(oldPublicId, newPublicId);

    var result = await _cloudinary.RenameAsync(renameParams);

    if (result.StatusCode != System.Net.HttpStatusCode.OK) return null;
    
    return result;
  }
  
  public async Task<ImageUploadResult> GetFilesAsync()
  {
    return null;
  }
}