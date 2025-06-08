using System.Text.Json;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MaClasse.Shared.Models.Files;
using MaClasse.Shared.Models.Lesson;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Service.Cloudinary.Interfaces;
using Service.Database.Services;

namespace Service.Cloudinary.Controllers;

[ApiController]  
[Route("api")]
public class CloudController : ControllerBase
{
  private readonly UserService _userService;
  private readonly ICloudRepository _fileRepository;
  private readonly CloudinaryDotNet.Cloudinary _cloudinary;
  private readonly VerifyDeleteService _verifyDeleteService;

  public CloudController(
    UserService userService,
    ICloudRepository fileRepository,
    CloudinaryDotNet.Cloudinary cloudinary,
    VerifyDeleteService verifyDeleteService)
  {
    _userService = userService;
    _fileRepository = fileRepository;
    _cloudinary = cloudinary;
    _verifyDeleteService = verifyDeleteService;
  }

  [HttpPost]
  [Route("add-file")]
  public async Task<IActionResult> AddFile(
    [FromForm] IFormFile file, [FromForm] string  filerequest)
  {
    // commentaire pour cloudinary
    var idSession = JsonSerializer.Deserialize<FileRequest>(filerequest);

    var idUser =
      _userService.GetUserByIdSession(idSession.IdSession).Result.UserId;

    var newFileResult =
      await _fileRepository.UploadFileAsync(file, idUser);
    
    //* Maintenant j'envoie les informations relatives à l'images vers la base de données
    var newDocument = new Document
    {
      IdDocument = ObjectId.GenerateNewId().ToString(),
      IdCloudinary = newFileResult.PublicId,
      Name = newFileResult.OriginalFilename,
      Url = newFileResult.SecureUrl.ToString(),
      ThumbnailUrl = _cloudinary.Api.UrlImgUp
        .Transform(new Transformation()
          .Width(100)
          .Crop("limit"))
        .BuildUrl($"{newFileResult.PublicId}.{newFileResult.Format}"),
      Format = newFileResult.Format,
      CreatedAt = newFileResult.CreatedAt   
    };
    
    Console.WriteLine($"PublicId: {newFileResult.PublicId}, Format: {newFileResult.Format}");
    Console.WriteLine($"Thumbnail URL: {_cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Crop("limit")).BuildUrl($"{newFileResult.PublicId}.{newFileResult.Format}")}");
    
    return Ok(newDocument);
  }

  [HttpPost]
  [Route("get-file")]
  public async Task<IActionResult> GetFile()
  {
    return Ok();
  }
  
  [HttpPost]
  [Route("rename-file")]
  public async Task<IActionResult> UpdateFile([FromBody] Document document)
  {
    //* Vérification qu'il existe ce document
    var existingDocument = await _fileRepository.GetFileAsyncByIdCloudinary(document.IdCloudinary);
    
    if (existingDocument == null) return NotFound();
    
    //* Format le nouveau nom de fichier
    var folderName = existingDocument.PublicId.Substring(0, existingDocument.PublicId.LastIndexOf('/'));
    var fileName = existingDocument.PublicId.Substring(existingDocument.PublicId.LastIndexOf('/') + 1);
    var suffix = fileName.Substring(fileName.LastIndexOf('_'));
    var newFileName = $"{folderName}/{document.Name}{suffix}";

    //* Renomme le document
    var RenamedDocument = await _fileRepository.RenameFileAsync(existingDocument.PublicId, newFileName);

    if (RenamedDocument == null) return BadRequest();

    document.IdCloudinary = newFileName;
    
    return Ok(document);
  }

  [HttpPost]
  [Route("delete-file")]
  public async Task<IActionResult> DeleteFile([FromBody] RequestLesson request)
  {
    //* Vérification qu'il existe ce document
    var existingDocument = await _fileRepository.GetFileAsyncByIdCloudinary(request.Document.IdCloudinary);

    if (existingDocument == null) return NotFound();

    //* Vérification que le document n'est pas utilisé dans une autre leçon
    var documentsToDelete = await _verifyDeleteService.VerifyDeleteFiles(request);
    
    //* Si le retour est vide cela signifie que le document est utilisé dans une autre leçon
    if(documentsToDelete.Count == 0)
    {
      //* On bypass la suppression du document
      return Ok();
    }
    
    //* Efface le document
    var deletedDocument = await _fileRepository.DeleteFileAsync(request.Document.IdCloudinary);
    return Ok(existingDocument);
  }
  
  [HttpPost]
  [Route("get-files")]
  public async Task<IActionResult> GetFiles()
  {
    return Ok();
  }
  
  [HttpPost]
  [Route("delete-files")]
  public async Task<IActionResult> DeleteFiles([FromBody] RequestLesson request)
  {
    var documentsToDelete = await _verifyDeleteService.VerifyDeleteFiles(request);
    
    foreach (var document in documentsToDelete)
    {
      var existingDocument = await _fileRepository.GetFileAsyncByIdCloudinary(document.IdCloudinary);
      
      if (existingDocument == null) return NotFound();
      
      await _fileRepository.DeleteFileAsync(document.IdCloudinary);
    
    }
    
    return Ok();
  }
}