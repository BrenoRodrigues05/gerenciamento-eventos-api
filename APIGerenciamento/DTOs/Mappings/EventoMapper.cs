using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;
using APIGerenciamento.DTOs.Mappings;


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
                Vagas = dto.Vagas,
                Cidade = dto.Cidade,
                Entrada = dto.Entrada ?? "Gratuita"
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
                Vagas = entity.Vagas,
                Cidade = entity.Cidade,
                Entrada = entity.Entrada ?? "Gratuita"
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
                Vagas = entity.Vagas,
                Cidade = entity.Cidade,
                Entrada = entity.Entrada ?? "Gratuita"
            };
        }

        public void PatchToEntity(EventoPatchDTO dto, Evento entity)
        {
            if (dto.Titulo != null) entity.Titulo = dto.Titulo;
            if (dto.Descricao != null) entity.Descricao = dto.Descricao;
            if (dto.Data.HasValue) entity.Data = dto.Data.Value;
            if (dto.Local != null) entity.Local = dto.Local;
            if (dto.Vagas.HasValue) entity.Vagas = dto.Vagas.Value;
            if (dto.Cidade != null) entity.Cidade = dto.Cidade;     
            if (dto.Entrada != null) entity.Entrada = dto.Entrada;
        }
    }
}
