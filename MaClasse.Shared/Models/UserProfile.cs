﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MaClasse.Shared.Models;

public class UserProfile
{
    [Required] [MaxLength(255)] public string Id { get; set; } = "";
    [Required] [EmailAddress] [MaxLength(255)] public string Email { get; set; } = "";
    [Required] [MaxLength(255)] public string Name { get; set; } = "";
    [MaxLength(255)] public string GivenName { get; set; } = "";
    [MaxLength(255)] public string FamilyName { get; set; } = "";
    [MaxLength(255)] public string Picture { get; set; } = "";
    [MaxLength(255)] public string? CreatedAt { get; set; }
    [MaxLength(255)] public string? UpdatedAt { get; set; }
}