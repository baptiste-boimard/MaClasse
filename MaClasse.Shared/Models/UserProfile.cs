using Microsoft.AspNetCore.Identity;

namespace MaClasse.Shared.Models;

public class UserProfile : IdentityUser<Guid>
{   
    //? Name est la donnée retournée par Google, mais en BDD
    //? je stocke dans UserName l'Email à la place du Name
    //? à cause de l'unicité de UserName
    
    public required string Name { get; set; }
    public required string Picture { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}