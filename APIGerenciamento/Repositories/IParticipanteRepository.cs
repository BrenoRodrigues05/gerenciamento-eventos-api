using APIGerenciamento.Models;
using APIGerenciamento.Pagination;

namespace APIGerenciamento.Repositories
{
    public interface IParticipanteRepository : IRepository<Participante>
    {
        Task<PagedResult<Participante>> GetFiltroNomesParticipantesAsync(FiltroNomeParticipante filtro);
    }
}
