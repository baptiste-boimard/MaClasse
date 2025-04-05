using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Service.OAuth.Interfaces;

namespace Service.OAuth.Controller;

[ApiController]
[Route("api")]
public class SessionController : ControllerBase
{
    private readonly ISessionRepository _sessionRepository;

    public SessionController(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    [HttpPost]
    [Route("logout-session")]
    public async Task<IActionResult> LogoutSession([FromBody] LogoutRequest request)
    {
        var existingSession = await _sessionRepository.GetUserIdByCookies(request.IdSession);

        if (existingSession != null)
        {
            await _sessionRepository.DeleteSessionData(existingSession);
            return Ok();
        }
        
        return NotFound();
    }
}