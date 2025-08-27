using APIGerenciamento.DTOs;
using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;

namespace APIGerenciamento.Services
{
    public class EventosCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDTOMapper<EventoDTO, Evento, EventoPatchDTO> _mapper;

        private const string CacheKey = "eventosCache";

        public EventosCacheService(IMemoryCache cache, IUnitOfWork unitOfWork, IDTOMapper<EventoDTO,
            Evento, EventoPatchDTO> mapper)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // cache para lista de eventos
        public async Task<IEnumerable<EventoDTO>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<EventoDTO> eventos))
            {
                var eventosFromDb = await _unitOfWork.Eventos.GetAllAsync();
                eventos = eventosFromDb.Select(e => _mapper.ToDto(e)).ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(CacheKey, eventos, cacheEntryOptions);
            }
            return eventos;
        }

        // cache para evento por id

        public async Task<EventoDTO?> GetEventoByIdAsync(int id)
        {
            var cacheKeyById = $"{CacheKey}_{id}";

            return await _cache.GetOrCreateAsync(cacheKeyById, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                var eventoFromDb = await _unitOfWork.Eventos.GetByIdAsync(id);
                return eventoFromDb != null ? _mapper.ToDto(eventoFromDb) : null;
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