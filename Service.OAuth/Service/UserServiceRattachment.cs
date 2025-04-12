using MaClasse.Shared.Models;
using Service.OAuth.Interfaces;
using Service.OAuth.Repositories;

namespace Service.OAuth.Service;

public class UserServiceRattachment
{
    private readonly IAuthRepository _authRepository;

    public UserServiceRattachment(
        IAuthRepository authRepository )
    {
        _authRepository = authRepository;
    }

    public async Task<AuthReturn?> GetUserWithRattachment(UserProfile user, bool isNewUser, string idSession)
    {
        var rattachments = await _authRepository.GetRattachmentByIdRole(user.IdRole);
        
        var userWithRattachment = new UserWithRattachment
        {
            UserProfile = user,
            AsDirecteur = rattachments.Where(r => r.IdDirecteur == user.IdRole).ToList(),
            AsProfesseur = rattachments.Where(r => r.IdProfesseur == user.IdRole).ToList()
        };

        var authReturn = new AuthReturn
        {
            IsNewUser = isNewUser,
            UserWithRattachment = userWithRattachment,
            IdSession = idSession
        };
        
        return authReturn;
    }
}