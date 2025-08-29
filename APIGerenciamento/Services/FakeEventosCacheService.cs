using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;

namespace APIGerenciamento.Services
{

    // Serviço Fake para testes unitários
    public class FakeEventosCacheService : EventosCacheService
    {
        public FakeEventosCacheService(IUnitOfWork unitOfWork, IDTOMapper<EventoDTO, Evento, EventoPatchDTO> mapper)
            : base(new MemoryCache(new MemoryCacheOptions()), unitOfWork, mapper)
        { }
    }
}
