using APIGerenciamento.DTOs;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Asp.Versioning;
using ApiVersion = Asp.Versioning.ApiVersion;
using ApiVersionAttribute = Microsoft.AspNetCore.Mvc.ApiVersionAttribute;

namespace APIGerenciamento.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class AuthControllerV2 : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Usuario> _usuarioRepository;

        public AuthControllerV2(AuthService authService, IUnitOfWork unitOfWork, IUsuarioRepository
            usuarioRepository)
        {
            _authService = authService;
            _unitOfWork = unitOfWork;
            _usuarioRepository = usuarioRepository;
        }

        public class LoginRequest
        {
            public string? Email { get; set; }
            public string? Senha { get; set; }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.Email!, request.Senha!);
            if (token == null)
                return Unauthorized("Credenciais inválidas");

            return Ok(new { token });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            var usuario = new Usuario
            {
                Email = dto.Email,
                Role = "User", // Role padrão ao criar um novo usuário
                SenhaHash = AuthService.HashSenha(dto.Senha)
            };

            await _usuarioRepository.AddAsync(usuario);
            await _unitOfWork.CommitAsync();

            return Ok(new { mensagem = "Usuário registrado com sucesso!" });
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPut("change-role/{usuarioId}")]
        public async Task<IActionResult> ChangeRole(int usuarioId, [FromBody] string novaRole)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                return NotFound("Usuário não encontrado");

            usuario.Role = novaRole;
            _usuarioRepository.Update(usuario);
            await _unitOfWork.CommitAsync();

            return Ok(new { mensagem = "Role alterada com sucesso" });
        }
    }
}
