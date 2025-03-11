using MaClasse.Shared.Models;
using Microsoft.AspNetCore.Identity;

namespace MaClasse.Client.Services;

public class ApplicationUser : IdentityUser
{
    // Vous pouvez ajouter des propriétés supplémentaires si nécessaire
    public string? FullName { get; set; }
}