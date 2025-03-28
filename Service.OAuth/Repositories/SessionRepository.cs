using MaClasse.Shared.Models;
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
    
}