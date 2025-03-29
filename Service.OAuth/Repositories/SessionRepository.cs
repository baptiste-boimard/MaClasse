using MaClasse.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Service.OAuth.Database;
using Service.OAuth.Interfaces;

namespace Service.OAuth.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly PostgresDbContext _postgresDbContext;

    public SessionRepository(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }
    public async Task<SessionData> GetUserIdByCookies(string token)
    {
        var user = await _postgresDbContext.SessionDatas.FirstOrDefaultAsync(
            s => s.Token == token);

        if (user != null) return user;
        
        return null;
    }

    public async Task<SessionData> SaveNewSession(SessionData sessionData)
    {
        _postgresDbContext.SessionDatas.Add(sessionData);
        await _postgresDbContext.SaveChangesAsync();
        return sessionData;
    }

    public async Task<SessionData> DeleteSessionData(SessionData sessionData)
    {
        return null;
    }

    public async Task<SessionData> UpdateSession(SessionData sessionData)
    {
        _postgresDbContext.Update(sessionData);
        await _postgresDbContext.SaveChangesAsync();
        
        return sessionData;
    }
    
}