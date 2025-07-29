using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;

namespace APIGerenciamento.DTOs.Mappings
{

    public class EventoMapper : IDTOMapper<EventoDTO, Evento, EventoPatchDTO>
    {
        public Evento ToEntity(EventoDTO dto)
        {
            return new Evento
            {
                Id = dto.Id,
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Data = dto.Data,
                Local = dto.Local,
                Vagas = dto.Vagas
            };
        }

        public EventoDTO ToDto(Evento entity)
        {
            return new EventoDTO
            {
                Id = entity.Id,
                Titulo = entity.Titulo,
                Descricao = entity.Descricao,
                Data = entity.Data,
                Local = entity.Local,
                Vagas = entity.Vagas
            };
        }

        public EventoPatchDTO ToPatchDto(Evento entity)
        {
            return new EventoPatchDTO
            {
                Titulo = entity.Titulo,
                Descricao = entity.Descricao,
                Data = entity.Data,
                Local = entity.Local,
                Vagas = entity.Vagas
            };
        }

        public void PatchToEntity(EventoPatchDTO dto, Evento entity)
        {
            if (dto.Titulo != null) entity.Titulo = dto.Titulo;
            if (dto.Descricao != null) entity.Descricao = dto.Descricao;
            if (dto.Data.HasValue) entity.Data = dto.Data.Value;
            if (dto.Local != null) entity.Local = dto.Local;
            if (dto.Vagas.HasValue) entity.Vagas = dto.Vagas.Value;
        }
    }
}
