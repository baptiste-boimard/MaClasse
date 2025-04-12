using MaClasse.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Service.OAuth.Database;
using Service.OAuth.Interfaces;

namespace Service.OAuth.Repositories;

public class RattachementRepository : IRattachmentRepository
{
    private readonly PostgresDbContext _postgresDbContext;

    public RattachementRepository(
        PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    public async Task<List<Rattachment>> GetRattachmentProf(string idRoleUser)
    {
        var listRattachment = await _postgresDbContext.Rattachments
            .Where(r => r.IdDirecteur == idRoleUser)
            .ToListAsync();
        
        return listRattachment;
    }

    public async Task<List<Rattachment>> GetRattachmentDirect(string IdRoleUser)
    {
        var listRattachment = await _postgresDbContext.Rattachments
            .Where(r => r.IdProfesseur == IdRoleUser)
            .ToListAsync();

        return listRattachment;
    }
    
    public async Task<List<Rattachment>> GetRattachment(Rattachment rattachment)
    {
        var existingRattachment = await _postgresDbContext.Rattachments
            .Where(r => r.IdProfesseur == rattachment.IdProfesseur && r.IdDirecteur == rattachment.IdDirecteur)
            .ToListAsync();

        return existingRattachment;
    }
    
    public async Task<Rattachment> AddRattachment(Rattachment rattachment)
    {
        //* Création de l'IdGuid
        rattachment.IdGuid = Guid.NewGuid();
        
        _postgresDbContext.Rattachments.Add(rattachment);
        await _postgresDbContext.SaveChangesAsync();
        
        return rattachment;
    }
    
}