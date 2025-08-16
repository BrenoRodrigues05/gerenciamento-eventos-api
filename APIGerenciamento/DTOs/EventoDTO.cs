using APIGerenciamento.Models;
using APIGerenciamento.Validations;
using System.ComponentModel.DataAnnotations;

namespace APIGerenciamento.DTOs
{
    public class EventoDTO
    {
       
        public int Id { get; set; }
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título pode ter no máximo 100 caracteres.")]
        [PrimeiraLetraMaiuscula(ErrorMessage = "O título deve começar com letra maiúscula.")]
        public string? Titulo { get; set; }
        [Required]
        [StringLength(500, ErrorMessage = "A descrição pode ter no máximo 500 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A data do evento é obrigatória.")]
        public DateTime Data { get; set; }
        [Required(ErrorMessage = "O local do evento é obrigatório.")]
        [StringLength(200, ErrorMessage = "O local pode ter no máximo 200 caracteres.")]
        public string? Local { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O número de vagas deve ser maior que zero.")]
        public int Vagas { get; set; }

        [Required(ErrorMessage = "O Estado é obrigatório")]
        [PrimeiraLetraMaiuscula(ErrorMessage = "O Estado deve começar com letra maiúscula.")]
        public string? Cidade { get; set; }

        [Required]
        public string? Entrada { get; set; } = "Gratuita"; // Entrada padrão como "Gratuita"


    }
}
