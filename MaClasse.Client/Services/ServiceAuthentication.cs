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
    }
    
    public void RemoveToken()
    {
        JwtToken = null;
    }
}