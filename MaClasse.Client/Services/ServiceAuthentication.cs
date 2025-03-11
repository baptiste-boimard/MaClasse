using System.Net.Http.Headers;

namespace MaClasse.Client.Services;

public class ServiceAuthentication
{
    public string? JwtToken { get; private set; }
    
    public void SetToken(string? token)
    {
        JwtToken = token;
    }

    public void AttachToken(HttpClient client)
    {
        if (!string.IsNullOrEmpty(JwtToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtToken);
        }
        
        client.DefaultRequestHeaders.Add("Cookie", "MaClasseAuth=VotreCookieValue");

    }
    
    public void RemoveToken()
    {
        JwtToken = null;
    }
    
    //! Ajout d'une méthode pour enlever le token des Headers
}