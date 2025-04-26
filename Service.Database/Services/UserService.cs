using MaClasse.Shared.Models;
using MaClasse.Shared.Models.Auth;

namespace Service.Database.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SessionData> GetUserByIdSession(string idSession)
    {

        var response = await _httpClient.PostAsJsonAsync(
            "https://localhost:7261/api/auth/get-user", new UserBySessionRequest
            {
                IdSession = idSession
            });
        
        if (response.IsSuccessStatusCode)
        {
            var userSession = await response.Content.ReadFromJsonAsync<SessionData>();
            
            //* je récupére l'id de l'utilisateur
            return userSession;
        }

        return null;
    }
}