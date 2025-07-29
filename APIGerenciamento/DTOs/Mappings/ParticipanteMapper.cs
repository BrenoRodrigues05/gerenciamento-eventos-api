using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;

namespace APIGerenciamento.DTOs.Mappings
{
    public class ParticipanteMapper : IDTOMapper<ParticipanteDTO, Participante, ParticipantePatchDTO>
    {
        public Participante ToEntity(ParticipanteDTO dto)
        {
            return new Participante
            {
                Id = dto.Id,
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone
            };
        }

        public ParticipanteDTO ToDto(Participante entity)
        {
            return new ParticipanteDTO
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Email = entity.Email,
                Telefone = entity.Telefone
            };
        }

        public ParticipantePatchDTO ToPatchDto(Participante entity)
        {
            return new ParticipantePatchDTO
            {
                Nome = entity.Nome,
                Email = entity.Email,
                Telefone = entity.Telefone
            };
        }

        public void PatchToEntity(ParticipantePatchDTO dto, Participante entity)
        {
            if (dto.Nome != null) entity.Nome = dto.Nome;
            if (dto.Email != null) entity.Email = dto.Email;
            if (dto.Telefone != null) entity.Telefone = dto.Telefone;
        }
    }
}
