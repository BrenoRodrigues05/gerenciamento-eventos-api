using APIGerenciamento.Context;
using APIGerenciamento.Models;
using APIGerenciamento.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APIGerenciamento.Repositories
{
    public class EventoRepository : Repository<Evento>, IEventoRepository
    {
        public EventoRepository(APIGerenciamentoContext ctx) : base(ctx) { }

        public async Task<PagedResult<Evento>> GetTituloEventoAsync(FiltroEventoTitulo tituloeventoparams)
        {
            var query = Query();

            if (!string.IsNullOrWhiteSpace(tituloeventoparams.Titulo))
            {
                query = query.Where(e => e.Titulo.ToLower().Contains(tituloeventoparams.Titulo.ToLower()));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((tituloeventoparams.PageNumber - 1) * tituloeventoparams.PageSize)
                .Take(tituloeventoparams.PageSize)
                .ToListAsync();

            return new PagedResult<Evento>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = tituloeventoparams.PageNumber,
                PageSize = tituloeventoparams.PageSize
            };
        }
    }
}
