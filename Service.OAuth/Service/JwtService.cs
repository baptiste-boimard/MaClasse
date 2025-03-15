using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MaClasse.Shared.Models;
using Microsoft.IdentityModel.Tokens;

namespace Service.OAuth.Service;

public class JwtService
{
    // private readonly IConfiguration _configuration;
    //
    // public JwtService(IConfiguration configuration)
    // {
    //     _configuration = configuration;
    // }
    //
    // public string GenerateJwtToken(UserProfile user) {
    //     
    //     var jwtKey = _configuration["Jwt:Key"];
    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //     
    //     var claims = new List<Claim>
    //     {
    //         new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    //         new Claim(JwtRegisteredClaimNames.Email, user.Email),
    //         new Claim(ClaimTypes.Name, user.UserName),
    //         new Claim("Picture", user.Picture),
    //         new Claim("CreatedAt", user.CreatedAt.ToString()),
    //         new Claim("UpdateddAt", user.UpdatedAt.ToString()),
    //         // Ajoutez d'autres claims si nécessaire, comme roles, etc.
    //     };
    //
    //     var token = new JwtSecurityToken(
    //         issuer: _configuration["Jwt:Issuer"],
    //         audience: _configuration["Jwt:Audience"],
    //         claims: claims,
    //         expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
    //         signingCredentials: creds);
    //
    //     return new JwtSecurityTokenHandler().WriteToken(token);
    // }
}