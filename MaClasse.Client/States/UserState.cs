namespace MaClasse.Client.States;

public class UserState
{
    public UserState()
    {
        
    }
    
    public string IdSession { get; set; }
    public string Id { get; set; } 
    public string Email { get; set; }
    public string Name { get; set; } 
    public string Role { get; set; } 
    public string Zone { get; set; }
    public string GivenName { get; set; } 
    public string FamilyName { get; set; } 
    public string Picture { get; set; } 
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void SetUser(UserState userState)
    {
        IdSession = userState.IdSession;
        Id = userState.Id;
        Email = userState.Email;
        Name = userState.Name;
        Role = userState.Role;
        Zone = userState.Zone;
        GivenName = userState.GivenName;
        FamilyName = userState.FamilyName;
        Picture = userState.Picture;
        CreatedAt = userState.CreatedAt;
        UpdatedAt = userState.UpdatedAt;
    }

    public UserState GetUser()
    {
        return this;
    }

    public void ResetUserState()
    {
        IdSession = "";
        Id = "";
        Email = "";
        Name = "";
        Role = "";
        Zone = "";
        GivenName = "";
        FamilyName = "";
        Picture = "";
        CreatedAt = null;
        UpdatedAt = null;
    }
}