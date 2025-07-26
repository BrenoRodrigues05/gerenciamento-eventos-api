using APIGerenciamento.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIGerenciamento.DTOs
{
    public class InscricaoDTO
    {
        public int Id { get; set; }

        [Required]
        public int EventoId { get; set; }

        public string? NomeEvento { get; set; }

        [Required]
        public int ParticipanteId { get; set; }

        public string? NomeParticipante { get; set; }

        public DateTime DataInscricao { get; set; } = DateTime.Now;
    }
}
