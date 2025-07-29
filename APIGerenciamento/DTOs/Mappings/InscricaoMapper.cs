using APIGerenciamento.DTOs.Patch;
using APIGerenciamento.Interfaces;
using APIGerenciamento.Models;

namespace APIGerenciamento.DTOs.Mappings
{
    public class InscricaoMapper : IDTOMapper<InscricaoDTO, Inscricao, InscricaoPatchDTO>
    {

        public Inscricao ToEntity(InscricaoDTO dto)
        {
            return new Inscricao
            {
                Id = dto.Id,
                EventoId = dto.EventoId,
                ParticipanteId = dto.ParticipanteId,
                DataInscricao = dto.DataInscricao
            };
        }

        public InscricaoDTO ToDto(Inscricao entity)
        {
            return new InscricaoDTO
            {
                Id = entity.Id,
                EventoId = entity.EventoId,
                ParticipanteId = entity.ParticipanteId,
                DataInscricao = entity.DataInscricao,
                NomeEvento = entity.Evento?.Titulo,
                NomeParticipante = entity.Participante?.Nome
            };
        }

        public InscricaoPatchDTO ToPatchDto(Inscricao entity)
        {
            return new InscricaoPatchDTO
            {
                EventoId = entity.EventoId,
                ParticipanteId = entity.ParticipanteId,
                DataInscricao = entity.DataInscricao
            };
        }

        public void PatchToEntity(InscricaoPatchDTO dto, Inscricao entity)
        {
            if (dto.EventoId.HasValue) entity.EventoId = dto.EventoId.Value;
            if (dto.ParticipanteId.HasValue) entity.ParticipanteId = dto.ParticipanteId.Value;
            if (dto.DataInscricao.HasValue) entity.DataInscricao = dto.DataInscricao.Value;
        }
    }
}
