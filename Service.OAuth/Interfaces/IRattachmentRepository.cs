using MaClasse.Shared.Models;

namespace Service.OAuth.Interfaces;

public interface IRattachmentRepository
{
    Task<List<Rattachment>> GetRattachmentProf(string idRoleUser);
    Task<List<Rattachment>> GetRattachmentDirect(string idRoleUser);
    Task<List<Rattachment>> GetRattachment(Rattachment rattachment);
    Task<Rattachment> AddRattachment(Rattachment rattachment);
    Task<Rattachment> DeleteRattachment(Rattachment rattachment);
}