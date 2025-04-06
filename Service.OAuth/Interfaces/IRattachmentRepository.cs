using MaClasse.Shared.Models;

namespace Service.OAuth.Interfaces;

public interface IRattachmentRepository
{
    Task<List<Rattachment>> GetRattachment(string idProfesseur);
    Task<Rattachment> AddRattachment(Rattachment rattachment);
}