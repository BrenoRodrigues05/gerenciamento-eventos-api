using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;

namespace APIGerenciamento.Services
{
    // Serviço Fake para testes unitários
    public class FakeParticipantesCacheService : ParticipanteCacheService
    {
        public FakeParticipantesCacheService(IUnitOfWork unitOfWork, IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> mapper)
            : base(new MemoryCache(new MemoryCacheOptions()), unitOfWork, mapper)
        { }
    }
   
}
