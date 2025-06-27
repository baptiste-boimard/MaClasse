using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Lesson;
using MaClasse.Shared.Models.Scheduler;
using Service.OAuth.Interfaces;
using Service.OAuth.Repositories;
using Service.OAuth.Service.Interface;

namespace Service.OAuth.Service;

public class UserServiceRattachment : IUserServiceRattachment
{
    private readonly IAuthRepository _authRepository;

    public UserServiceRattachment(
        IAuthRepository authRepository )
    {
        _authRepository = authRepository;
    }

    public async Task<AuthReturn?> GetUserWithRattachment(
        UserProfile user,
        bool isNewUser,
        string idSession,
        string? AccesToken, 
        Scheduler scheduler)
    {
        var rattachments = await _authRepository.GetRattachmentByIdRole(user.IdRole);
        
        var userWithRattachment = new UserWithRattachment
        {
            UserProfile = user,
            AsDirecteur = rattachments.Where(r => r.IdDirecteur == user.IdRole).ToList(),
            AsProfesseur = rattachments.Where(r => r.IdProfesseur == user.IdRole).ToList(),
            AccessToken = AccesToken
        };
        
        var authReturn = new AuthReturn
        {
            IsNewUser = isNewUser,
            UserWithRattachment = userWithRattachment,
            IdSession = idSession,
            Scheduler = scheduler,
        };
        
        return authReturn;
    }
}