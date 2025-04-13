using MaClasse.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Service.OAuth.Database;
using Service.OAuth.Interfaces;

namespace Service.OAuth.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly PostgresDbContext _postgresDbContext;

    public AuthRepository(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    public async Task<UserProfile?> GetOneUserByGoogleId(string googleId)
    {
        var user = await _postgresDbContext.UserProfiles.FirstOrDefaultAsync(
            u => u.Id == googleId);

        if (user != null)
        {
            return user;
        }

        return null;
    }

    public async Task<UserProfile> AddUser(UserProfile user)
    {
        var newUser = new UserProfile
        {
            Id = user.Id,
            IdRole = user.IdRole,
            Email = user.Email,
            Name = user.Name,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Picture = user.Picture,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _postgresDbContext.UserProfiles.Add(newUser);
        await _postgresDbContext.SaveChangesAsync();
        return newUser;
    }

    public async Task<UserProfile> UpdateUser(UserProfile user)
    {
        _postgresDbContext.UserProfiles.Update(user);
        await _postgresDbContext.SaveChangesAsync();
        
        return user;
    }

    public async Task<bool> CheckIdRole(string idRole)
    {
        var existingIdRole = 
            await _postgresDbContext.UserProfiles.FirstOrDefaultAsync(
                u => u.IdRole == idRole);
        
        if(existingIdRole == null)
        {
            return false;
        }
        return true;
    }

    public async Task<List<Rattachment>> GetRattachmentByIdRole(string IdRole)
    {
        var rattachments = await _postgresDbContext.Rattachments
            .Where(r => r.IdDirecteur == IdRole || r.IdProfesseur == IdRole)
            .ToListAsync();

        return rattachments;
    }

    public async Task<UserProfile> DeleteUser(UserProfile user)
    {
        _postgresDbContext.UserProfiles.Remove(user);
        await _postgresDbContext.SaveChangesAsync();

        return user;
    } 
}