using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Service.Cloudinary.Interfaces;

namespace Service.Cloudinary.Repositories;

public class FileRepository : IFileRepository
{
  private readonly CloudinaryDotNet.Cloudinary _cloudinary;

  public FileRepository(CloudinaryDotNet.Cloudinary cloudinary)
  {
    _cloudinary = cloudinary;
  }
  
  public async Task<ImageUploadResult> UploadFileAsync(IFormFile file, string folder = "default-folder")
  {
    if (file == null || file.Length == 0)
      throw new ArgumentException("Fichier invalide");

    await using var memoryStream = new MemoryStream();
    await file.CopyToAsync(memoryStream);
    memoryStream.Position = 0;

    var uploadParams = new ImageUploadParams
    {
      File = new FileDescription(file.FileName, memoryStream),
      Folder = "uploads", // ou ta structure de dossiers Cloudinary
      UseFilename = true,
      UniqueFilename = true,
      Overwrite = false
    };

    return await _cloudinary.UploadAsync(uploadParams);
  }
}