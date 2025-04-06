using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Service.OAuth.Interfaces;

namespace Service.OAuth.Controller;

[ApiController]
[Route("api")]
public class RattachmentController: ControllerBase
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IRattachmentRepository _rattachmentRepository;

    public RattachmentController(
        ISessionRepository sessionRepository,
        IAuthRepository authRepository,
        IRattachmentRepository rattachmentRepository)
    {
        _sessionRepository = sessionRepository;
        _authRepository = authRepository;
        _rattachmentRepository = rattachmentRepository;
    }
    
    [HttpPost]
    [Route("add-rattachment")]
    public async Task<IActionResult> AddRattachment([FromBody] RattachmentRequest request)
    {
        var existingSession = await _sessionRepository.GetUserIdByCookies(request.IdSession);

        if (existingSession != null)
        {
            var user = await _authRepository.GetOneUserByGoogleId(existingSession.UserId);

            if (user != null)
            {
                if (request.IdDirecteur != null)
                {
                    //* J'ajoute dans la table Rattachment l'idRole du user et l'idDirecteur
                    var rattachment = new Rattachment
                    {
                        IdProfesseur = user.Id,
                        IdDirecteur = request.IdDirecteur
                    };
                    
                    var addRattachment = await _rattachmentRepository.AddRattachment(rattachment); 
                    
                    if (addRattachment != null)
                    {
                        //* On renvoi la liste des directeur mise a jour
                        var listRattachment = await _rattachmentRepository.GetRattachment(user.Id);
                        
                        return Ok(addRattachment);
                    }
                }
                
                
                
                
                return Unauthorized();
            }
            
            return Unauthorized();
        }
        else
        {
            return Unauthorized();   
        }
    }
}