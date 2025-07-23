using APIGerenciamento.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIGerenciamento.Models
{
    [Table("Inscricao")]
    public class Inscricao : IEntidade
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EventoId { get; set; }
        public Evento? Evento { get; set; }
        [Required]  
        public int ParticipanteId { get; set; }
        public Participante? Participante { get; set; }

        public DateTime DataInscricao { get; set; } = DateTime.Now;
    }
}
