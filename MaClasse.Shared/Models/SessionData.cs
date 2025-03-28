namespace MaClasse.Shared.Models;

public class SessionData
{
    public string Token { get; set; }
    public string UserId { get; set; }
    public string Role { get; set; }
    public DateTime Expiration { get; set; }
}