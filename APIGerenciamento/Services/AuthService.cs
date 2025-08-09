using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace APIGerenciamento.Services
{
    public class AuthService
    {
        private readonly ConfigService _configService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(ConfigService configService, IUnitOfWork unitOfWork, IUsuarioRepository usuarioRepository)
        {
            _configService = configService;
            _unitOfWork = unitOfWork;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
        {
            return await _usuarioRepository.GetByEmailAsync(email);
        }

        public async Task<Usuario?> RegisterAsync(string email, string senha, string role)
        {
            var existe = await _usuarioRepository.GetByEmailAsync(email);
            if (existe != null)
                return null;

            var usuario = new Usuario
            {
                Email = email,
                SenhaHash = HashSenha(senha),
                Role = role
            };

            await _usuarioRepository.AddAsync(usuario);
            await _unitOfWork.CommitAsync();

            return usuario;
        }

        public async Task<string?> LoginAsync(string email, string senha)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario == null) return null;

            if (!VerificarSenha(senha, usuario.SenhaHash))
                return null;

            return GerarToken(usuario.Id.ToString(), usuario.Email, usuario.Role);
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

        public static string HashSenha(string senha)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: senha,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public static bool VerificarSenha(string senha, string senhaHash)
        {
            var partes = senhaHash.Split('.');
            if (partes.Length != 2) return false;

            var salt = Convert.FromBase64String(partes[0]);
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: senha,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32));

            return hash == partes[1];
        }
    }
}
