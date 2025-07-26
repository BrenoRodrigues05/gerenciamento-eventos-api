using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;

namespace APIGerenciamento.DTOs.Mappings
{
    public class InscricaoMapper : IDTOMapper<InscricaoDTO, Inscricao>
    {
        public InscricaoDTO ToDto(Inscricao entity)
        {
            return new InscricaoDTO
            {
                Id = entity.Id,
                EventoId = entity.EventoId,
                ParticipanteId = entity.ParticipanteId,
                DataInscricao = entity.DataInscricao,
                
            };
        }

        public Inscricao ToEntity(InscricaoDTO dto)
        {
            return new Inscricao
            {
                Id = dto.Id,
                EventoId = dto.EventoId,
                ParticipanteId = dto.ParticipanteId,
                DataInscricao = dto.DataInscricao,
            };
        }
    }
}
