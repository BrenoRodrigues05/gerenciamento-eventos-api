namespace APIGerenciamento.Services
{
    public class ConfigService
    {
        private readonly IConfiguration _configuration;

        public ConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection") ?? throw new 
                Exception("String de conexão não encontrada.");
        }

        public string GetJwtSecret()
        {
            return _configuration["JwtSettings:SecretKey"] ?? throw new Exception("Chave JWT não configurada.");
        }

        public string GetJwtIssuer()
        {
            return _configuration["JwtSettings:Issuer"] ?? "APIGerenciamento";
        }

        public string GetJwtAudience()
        {
            return _configuration["JwtSettings:Audience"] ?? "APIGerenciamento";
        }
    }
}

