using System.Text.Json;
using CloudinaryDotNet;
using MaClasse.Shared.Models.Files;
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
  private readonly DatabaseService _databaseService;

  public CloudController(
    UserService userService,
    ICloudRepository fileRepository,
    CloudinaryDotNet.Cloudinary cloudinary,
    DatabaseService databaseService)
  {
    _userService = userService;
    _fileRepository = fileRepository;
    _cloudinary = cloudinary;
    _databaseService = databaseService;
  }

  [HttpPost]
  [Route("add-file")]
  public async Task<IActionResult> AddFile(
    [FromForm] IFormFile file, [FromForm] string  filerequest)
  {
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
        .Transform(new Transformation().Width(150).Height(150).Crop("thumb").Gravity("auto"))
        .BuildUrl($"{newFileResult.PublicId}.{newFileResult.Format}"),
      Format = newFileResult.Format,
      CreatedAt = newFileResult.CreatedAt   
    };
    
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
  public async Task<IActionResult> DeleteFile([FromBody] Document document)
  {
    //* Vérification qu'il existe ce document
    var existingDocument = await _fileRepository.GetFileAsyncByIdCloudinary(document.IdCloudinary);

    if (existingDocument == null) return NotFound();
    
    //* Efface le document
    var deletedDocument = await _fileRepository.DeleteFileAsync(document.IdCloudinary);
    return Ok(existingDocument);
  }
  
  [HttpPost]
  [Route("get-files")]
  public async Task<IActionResult> GetFiles()
  {
    return Ok();
  }
}