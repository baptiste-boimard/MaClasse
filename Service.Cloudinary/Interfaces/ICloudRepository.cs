using System.Reflection.Metadata;
using CloudinaryDotNet.Actions;

namespace Service.Cloudinary.Interfaces;

public interface ICloudRepository
{
  Task<ImageUploadResult> UploadFileAsync(IFormFile file, string iduser);
  Task<GetResourceResult> GetFileAsyncByIdCloudinary(string idCloudinary);
  Task<ImageUploadResult> UpdateFileAsync();
  Task<DelResResult> DeleteFileAsync(string idCloudinary);
  Task<RenameResult> RenameFileAsync(string oldPublicId, string newPublicId);
  Task<ImageUploadResult> GetFilesAsync();
}