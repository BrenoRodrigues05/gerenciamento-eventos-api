using APIGerenciamento.Models;
using APIGerenciamento.Pagination;

namespace APIGerenciamento.Repositories
{
    public interface IEventoRepository : IRepository<Evento>
    {
        Task<PagedResult<Evento>> GetTituloEventoAsync(FiltroEventoTitulo tituloeventoparams);
    }
}
