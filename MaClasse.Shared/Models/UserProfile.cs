using Microsoft.AspNetCore.Identity;

namespace MaClasse.Shared.Models;

public class UserProfile : IdentityUser<Guid>
{
    public required string Name { get; set; }
    public required string Picture { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}