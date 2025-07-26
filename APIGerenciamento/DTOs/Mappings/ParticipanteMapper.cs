using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;

namespace APIGerenciamento.DTOs.Mappings
{
    public class ParticipanteMapper : IDTOMapper<ParticipanteDTO, Participante>
    {
        public ParticipanteDTO ToDto(Participante entity)
        {
            return new ParticipanteDTO
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Email = entity.Email,
                Telefone = entity.Telefone,
               
            };
        }

        public Participante ToEntity(ParticipanteDTO dto)
        {
            return new Participante
            {
                Id = dto.Id,
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
            };
        }
    }
}
