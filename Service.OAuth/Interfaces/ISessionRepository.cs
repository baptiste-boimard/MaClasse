﻿using MaClasse.Shared.Models;

namespace Service.OAuth.Interfaces;

public interface ISessionRepository
{
    Task<SessionData> GetUserIdByCookies(string token);
    Task<SessionData> SaveNewSession(SessionData sessionData);
    Task<SessionData> DeleteSessionData(SessionData sessionData);
    Task<SessionData> UpdateSession(SessionData sessionData);
    Task<SessionData> GetSessionByUserId(string userId);
}