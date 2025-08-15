using APIGerenciamento.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using APIGerenciamento.DTOs;
using Asp.Versioning;

namespace APIGerenciamento.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação e registro de usuários.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Mvc.ApiVersion("1.0", Deprecated = true)]
    public class AuthControllerV1 : ControllerBase
    {
        private readonly AuthService _authService;

        /// <summary>
        /// Construtor do controller de autenticação.
        /// </summary>
        /// <param name="authService">Serviço de autenticação</param>
        public AuthControllerV1(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// DTO interno para requisição de login.
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// Email do usuário.
            /// </summary>
            public string? Email { get; set; }

            /// <summary>
            /// Senha do usuário.
            /// </summary>
            public string? Senha { get; set; }
        }

        /// <summary>
        /// Realiza o login do usuário e retorna um token JWT.
        /// </summary>
        /// <param name="request">Objeto contendo email e senha do usuário.</param>
        /// <returns>Token JWT em caso de sucesso ou erro de autenticação.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.Email!, request.Senha!);
            if (token == null)
                return Unauthorized("Credenciais inválidas");

            return Ok(new { token });
        }

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        /// <param name="request">Objeto contendo email e senha para registro.</param>
        /// <returns>Dados do usuário registrado ou mensagem de erro.</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOs.RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _authService.GetUsuarioByEmailAsync(request.Email);
            if (existingUser != null)
                return Conflict("Usuário com este email já existe.");

            var usuario = await _authService.RegisterAsync(request.Email, request.Senha, "User");

            if (usuario == null)
                return StatusCode(500, "Erro ao criar usuário.");

            return Ok(new { usuario.Id, usuario.Email });
        }
    }
}
