namespace MaClasse.Shared.Models;

public class AuthReturn
{
    public bool IsNewUser { get; set; }
    public UserWithRattachment UserWithRattachment { get; set; }
    public string IdSession { get; set; }
}