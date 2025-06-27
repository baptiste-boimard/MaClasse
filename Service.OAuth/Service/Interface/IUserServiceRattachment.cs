using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Scheduler;

namespace Service.OAuth.Service.Interface;

public interface IUserServiceRattachment
{
  Task<AuthReturn?> GetUserWithRattachment(UserProfile user,bool isNewUser,string idSession,string? AccesToken,Scheduler scheduler);
}