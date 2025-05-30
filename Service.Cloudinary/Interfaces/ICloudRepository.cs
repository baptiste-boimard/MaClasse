using CloudinaryDotNet.Actions;

namespace Service.Cloudinary.Interfaces;

public interface ICloudRepository
{
  Task<ImageUploadResult> UploadFileAsync(IFormFile file, string iduser);
  Task<ImageUploadResult> GetFileAsync();
  Task<ImageUploadResult> UpdateFileAsync();
  Task<ImageUploadResult> DeleteFileAsync();
  Task<ImageUploadResult> GetFilesAsync();
}