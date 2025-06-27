using Service.OAuth.Interfaces;
using Service.OAuth.Repositories;
using Service.OAuth.Service.Interface;

namespace Service.OAuth.Service;

public class GenerateIdRole : IGenerateIdRole
{
    private readonly IAuthRepository _authRepository;

    public GenerateIdRole(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }
    
    public async Task<string> GenerateIdAsync(int length = 6)
    {
        while (true)
        {
            var candidate = Generate(length);

            var isFree = await VerifyExistingIdRole(candidate);
            if (isFree)
                return candidate;
        }
    }

    public string Generate(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<bool> VerifyExistingIdRole(string idRole)
    {
        var existing = await _authRepository.CheckIdRole(idRole);
        
        if (!existing) return true;
        
        return false;
    }
}