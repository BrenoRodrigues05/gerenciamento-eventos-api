using APIGerenciamento.Context;
using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Mappings;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Filters;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Logging;
using APIGerenciamento.Middlewares;
using APIGerenciamento.Models;
using APIGerenciamento.Repositories;
using APIGerenciamento.Services;
using APIGerenciamento.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura��o do banco de dados
var mysqlconnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<APIGerenciamentoContext>(options =>
    options.UseMySql(mysqlconnection, ServerVersion.AutoDetect(mysqlconnection)));

// Inje��o de depend�ncias
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ConfigService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO>, ParticipanteMapper>();
builder.Services.AddScoped<IDTOMapper<EventoDTO, Evento, EventoPatchDTO>, EventoMapper>();
builder.Services.AddScoped<IDTOMapper<InscricaoDTO, Inscricao, InscricaoPatchDTO>, InscricaoMapper>();

// Filtros e Serializa��o JSON
builder.Services.AddControllers(options =>
{
    options.Filters.Add<APILoggingFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true;
}).AddNewtonsoftJson();

// Logger customizado
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));


// Autentica��o JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var config = builder.Configuration;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["JwtSettings:Issuer"],
            ValidAudience = config["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]))
        };
    });

var app = builder.Build();

// Middleware de exce��es
app.UseMiddleware<ExceptionMiddleware>();

// Swagger no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
