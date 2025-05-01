using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MaClasse.Shared.Models;

public class UserProfile
{
    [Key] [Required] [MaxLength(255)] public string Id { get; set; } = "";
    [Required] [MaxLength(255)] public string IdRole { get; set; } = "";
    [Required] [EmailAddress] [MaxLength(255)] public string Email { get; set; } = "";
    [Required] [MaxLength(255)] public string Name { get; set; } = "";
    [Required] [MaxLength(255)] public string Role { get; set; } = "";
    [Required] [MaxLength(255)] public string Zone { get; set; } = "";
    [MaxLength(255)] public string GivenName { get; set; } = "";
    [MaxLength(255)] public string FamilyName { get; set; } = "";
    [MaxLength(255)] public string Picture { get; set; } = "";
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}