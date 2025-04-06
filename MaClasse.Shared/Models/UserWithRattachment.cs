namespace MaClasse.Shared.Models;

public class UserWithRattachment
{
    public UserProfile UserProfile { get; set; }
    public List<Rattachment> AsDirecteur { get; set; }
    public List<Rattachment> AsProfesseur { get; set; }
}