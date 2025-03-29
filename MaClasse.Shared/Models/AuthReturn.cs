namespace MaClasse.Shared.Models;

public class AuthReturn
{
    public bool IsNewUser { get; set; }
    public UserProfile? User { get; set; }
    public string IdSession { get; set; }
}