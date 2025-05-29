using CloudinaryDotNet.Actions;

namespace Service.Cloudinary.Interfaces;

public interface IFileRepository
{
  Task<ImageUploadResult> UploadFileAsync(IFormFile file, string folder = "default-folder");
}