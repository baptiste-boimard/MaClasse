using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Service.Cloudinary.Interfaces;

namespace Service.Cloudinary.Repositories;

public class CloudRepository : ICloudRepository
{
  private readonly CloudinaryDotNet.Cloudinary _cloudinary;

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

  public async Task<ImageUploadResult> GetFileAsync()
  {
    return null;
  }
  
  public async Task<ImageUploadResult> UpdateFileAsync()
  {
    return null;
  }
  
  public async Task<ImageUploadResult> DeleteFileAsync()
  {
    return null;
  }
  
  public async Task<ImageUploadResult> GetFilesAsync()
  {
    return null;
  }
}