using MaClasse.Shared.Models;

namespace MaClasse.Client.States;

public class UserState
{
    public UserState()
    {

    }

    public event Action OnChange;

    public string IdSession { get; set; }
    public string Id { get; set; }
    public string IdRole { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string Zone { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Picture { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<Rattachment> AsDirecteur { get; set; }
    public List<Rattachment> AsProfesseur { get; set; }


    public void SetUser(UserState userState)
    {
        IdSession = userState.IdSession;
        Id = userState.Id;
        IdRole = userState.IdRole;
        Email = userState.Email;
        Name = userState.Name;
        Role = userState.Role;
        Zone = userState.Zone;
        GivenName = userState.GivenName;
        FamilyName = userState.FamilyName;
        Picture = userState.Picture;
        CreatedAt = userState.CreatedAt;
        UpdatedAt = userState.UpdatedAt;
        AsDirecteur = userState.AsDirecteur;
        AsProfesseur = userState.AsProfesseur;
    }

    public UserState GetUser()
    {
        return this;
    }

    public void ResetUserState()
    {
        IdSession = "";
        Id = "";
        IdRole = "";
        Email = "";
        Name = "";
        Role = "";
        Zone = "";
        GivenName = "";
        FamilyName = "";
        Picture = "";
        CreatedAt = null;
        UpdatedAt = null;
        AsDirecteur = new List<Rattachment>();
        AsProfesseur = new List<Rattachment>();
    }

    public List<Rattachment> SetAsDirecteur(List<Rattachment> rattachments)
    {
        AsDirecteur = rattachments;
        NotifyStateChanged();
        return AsDirecteur;
    }
    
    public List<Rattachment> SetAsProfesseur(List<Rattachment> rattachments)
    {
        AsProfesseur = rattachments;
        NotifyStateChanged();
        return AsProfesseur;
    }
    
    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}