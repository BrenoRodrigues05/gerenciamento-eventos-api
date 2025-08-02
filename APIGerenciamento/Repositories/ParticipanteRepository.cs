using APIGerenciamento.Context;
using APIGerenciamento.Models;
using APIGerenciamento.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APIGerenciamento.Repositories
{
    public class ParticipanteRepository : Repository<Participante>, IParticipanteRepository
    {
        public ParticipanteRepository(APIGerenciamentoContext ctx) : base(ctx) { }

        public async Task<PagedResult<Participante>> GetFiltroNomesParticipantesAsync(FiltroNomeParticipante filtro)
        {
            var query = Query();

            if (!string.IsNullOrEmpty(filtro.Nome))
            {
                query = query.Where(p => p.Nome.ToLower().Contains(filtro.Nome.ToLower()));
            }
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync();

            return new PagedResult<Participante>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = filtro.PageNumber,
                PageSize = filtro.PageSize
            };
        }
    }

}
