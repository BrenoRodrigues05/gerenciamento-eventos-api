using APIGerenciamento.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using APIGerenciamento.DTOs;


namespace APIGerenciamento.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
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
        public async Task<IActionResult> Register([FromBody] DTOs.RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _authService.GetUsuarioByEmailAsync(request.Email);
            if (existingUser != null)
                return Conflict("Usuário com este email já existe.");

            var usuario = await _authService.RegisterAsync(request.Email, request.Senha, request.Role ?? "User");

            if (usuario == null)
                return StatusCode(500, "Erro ao criar usuário.");

            return Ok(new { usuario.Id, usuario.Email });
        }

    }
}

