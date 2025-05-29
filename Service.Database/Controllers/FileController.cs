using System.Text.Json;
using MaClasse.Shared.Models.Files;
using Microsoft.AspNetCore.Mvc;
using Service.Database.Services;

namespace Service.Database.Controllers;

[ApiController]  
[Route("api")]  
public class FileController : ControllerBase
{
  private readonly UserService _userService;

  public FileController(UserService userService)
  {
    _userService = userService;
  }

  [HttpPost]
  [Route("add-file")]
  public async Task<IActionResult> AddFile([FromForm] IFormFile file, [FromForm] string  filerequest)
  {
    var idSession = JsonSerializer.Deserialize<FileRequest>(filerequest);

    var idUser = _userService.GetUserByIdSession(idSession.IdSession).Result.UserId;
    
    return Ok();
  }
    
    
    
    
}