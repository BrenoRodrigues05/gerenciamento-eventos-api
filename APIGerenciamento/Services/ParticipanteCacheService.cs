using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;

namespace APIGerenciamento.Services
{
    public class ParticipanteCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO> _mapper;

        private const string CacheKey = "participantesCache";

        public ParticipanteCacheService(IMemoryCache cache, IUnitOfWork unitOfWork, IDTOMapper<ParticipanteDTO,
            Participante, ParticipantePatchDTO> mapper)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // cache para lista de participantes

        public async Task<IEnumerable<ParticipanteDTO>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<ParticipanteDTO> participantes))
            {
                var participantesFromDb = await _unitOfWork.Participantes.GetAllAsync();
                participantes = participantesFromDb.Select(p => _mapper.ToDto(p)).ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(CacheKey, participantes, cacheEntryOptions);
            }
            return participantes;
        }

        // cache para participante por id

        public async Task<ParticipanteDTO?> GetParticipanteByIdAsync(int id)
        {
            var cacheKeyById = $"{CacheKey}_{id}";
            return await _cache.GetOrCreateAsync(cacheKeyById, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var participanteFromDb = await _unitOfWork.Participantes.GetByIdAsync(id);
                return participanteFromDb != null ? _mapper.ToDto(participanteFromDb) : null;
            });
        }

        public void InvalidateCache(int? id = null)
        {
            _cache.Remove(CacheKey);
            if (id.HasValue)
            {
                var cacheKeyById = $"{CacheKey}_{id.Value}";
                _cache.Remove(cacheKeyById);
            }
        }
    }
}
