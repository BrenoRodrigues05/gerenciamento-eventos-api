using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;

namespace APIGerenciamento.DTOs.Mappings
{
    public class EventoMapper : IDTOMapper<EventoDTO, Evento>
    {
        public Evento ToEntity(EventoDTO dto)
        {
           return new Evento
            {
                Id = dto.Id,
                Titulo = dto.Titulo,
                Data = dto.Data,
                Local = dto.Local,
                Descricao = dto.Descricao
            };
        }
        public EventoDTO ToDto(Evento entity)
        {
            return new EventoDTO
            {
                Id = entity.Id,
                Titulo = entity.Titulo,
                Data = entity.Data,
                Local = entity.Local,
                Descricao = entity.Descricao
            };
        }

       
    }
}
