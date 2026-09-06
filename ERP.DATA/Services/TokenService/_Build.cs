using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ERP.TRAN.CrossLayers.API.Users.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ERP.DATA.Services.TokenService;

public partial class TokenManager(IConfiguration configuration)
{
    public string GenerarToken(int userId, string usuario, UserRole rol)
    {
        // Claims fijas que pediste: userId y rol, disponibles en cualquier
        // endpoint autenticado vía User.FindFirstValue(...).
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, rol.ToString()),
            new(ClaimTypes.Name, usuario)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiracionHoras = double.TryParse(configuration["Jwt:ExpiracionHoras"], out var horas) ? horas : 8;

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiracionHoras),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
}

