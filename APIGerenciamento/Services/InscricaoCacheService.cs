using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;

namespace APIGerenciamento.Services
{
    public class InscricaoCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        
        private const string CacheKey = "inscricoesCache";

        public InscricaoCacheService(IMemoryCache cache, IUnitOfWork unitOfWork)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
        }

        // cache para lista de inscrições

        public async Task<IEnumerable<Inscricao>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<Models.Inscricao> inscricoes))
            {
                inscricoes = await _unitOfWork.Inscricoes.GetAllAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(CacheKey, inscricoes, cacheEntryOptions);
            }
            return inscricoes;
        }

        public void InvalidateCache()
        {
            _cache.Remove(CacheKey);
        }
    }
}
