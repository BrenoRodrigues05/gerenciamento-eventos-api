using APIGerenciamento.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Exemplo fixo só para testes:

            if (request.Email == "admin@teste.com" && request.Senha == "123")
            {
                var token = _authService.GerarToken("1", request.Email, "Admin");
                return Ok(new { token });
            }

            return Unauthorized("Credenciais inválidas");
        }
    }
}

