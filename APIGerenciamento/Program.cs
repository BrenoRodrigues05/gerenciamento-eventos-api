using APIGerenciamento.Context;
using APIGerenciamento.Filters;
using APIGerenciamento.Logging;
using APIGerenciamento.Repositories;
using APIGerenciamento.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mysqlconnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<APIGerenciamentoContext>(options =>
    options.UseMySql(mysqlconnection, ServerVersion.AutoDetect(mysqlconnection)));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddControllers(options =>
{
    options.Filters.Add<APILoggingFilter>();
});
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
  
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
