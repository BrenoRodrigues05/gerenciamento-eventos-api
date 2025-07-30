using APIGerenciamento.DTOs;
using APIGerenciamento.Pagination;
using APIGerenciamento.UnitOfWork;
using System.Formats.Asn1;

namespace APIGerenciamento.Services
{
    public class EventosService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventosService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PagedResult<EventoDTO>> GetPaginadosAsync(int pageNumber, int pageSize)
        {
            var query = _unitOfWork.Eventos.Query();

            return await PaginationHelp.CreateAsync(query, pageNumber, pageSize, e => new EventoDTO
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Data = e.Data,
                Local = e.Local,
                Descricao = e.Descricao,
                Vagas = e.Vagas
            });
        }
    }
}
