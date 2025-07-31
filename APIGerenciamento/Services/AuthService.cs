using APIGerenciamento.Services;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIGerenciamento.Services
{
    public class AuthService
    {
        private readonly ConfigService _configService;

        public AuthService(ConfigService config)
        {
            _configService = config;
        }

        public string GerarToken(string usuarioId, string email, string role)
        {

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub, usuarioId),
               new Claim(JwtRegisteredClaimNames.Email, email),
               new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configService.GetJwtSecret()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               issuer: _configService.GetJwtIssuer(),
               audience: _configService.GetJwtAudience(),
               claims: claims,
               expires: DateTime.UtcNow.AddHours(1),
               signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
