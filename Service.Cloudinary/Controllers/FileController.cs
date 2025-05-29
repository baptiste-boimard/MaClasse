using System.Text.Json;
using MaClasse.Shared.Models.Files;
using Microsoft.AspNetCore.Mvc;
using Service.Cloudinary.Interfaces;
using Service.Cloudinary.Repositories;
using Service.Database.Services;

namespace Service.Cloudinary.Controller;

[ApiController]  
[Route("api")]
public class FileController : ControllerBase
{
  private readonly UserService _userService;
  private readonly IFileRepository _fileRepository;

  public FileController(
    UserService userService,
    IFileRepository fileRepository)
  {
    _userService = userService;
    _fileRepository = fileRepository; 
  }

  [HttpPost]
  [Route("add-file")]
  public async Task<IActionResult> AddFile([FromForm] IFormFile file, [FromForm] string  filerequest)
  {
    var idSession = JsonSerializer.Deserialize<FileRequest>(filerequest);

    var idUser = _userService.GetUserByIdSession(idSession.IdSession).Result.UserId;

    var newFile = _fileRepository.UploadFileAsync(file);
    return Ok();
  }
}