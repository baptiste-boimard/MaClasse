using MaClasse.Shared.Models;

namespace Service.OAuth.Interfaces;

public interface IAuthRepository
{
    Task<UserProfile> GetOneUserByGoogleId(string googleId);
    Task<UserProfile> AddUser(UserProfile user);
    Task<UserProfile> UpdateUser(UserProfile user);
    Task<bool> CheckIdRole(string idRole);
    Task<List<Rattachment>> GetRattachmentByIdRole(string idRole);
}