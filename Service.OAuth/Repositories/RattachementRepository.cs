using MaClasse.Shared.Models;
using Service.OAuth.Interfaces;

namespace Service.OAuth.Repositories;

public class RattachementRepository : IRattachmentRepository
{
    public RattachementRepository()
    {
        
    }

    public Task<List<Rattachment>> GetRattachment(string idProfesseur)
    {
        return null;
    }
    
    public Task<Rattachment> AddRattachment(Rattachment rattachment)
    {
        return null;
    }
    
}