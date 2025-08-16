using APIGerenciamento.Validations;
using System.ComponentModel.DataAnnotations;

namespace APIGerenciamento.DTOs.Patch
{
    public class EventoPatchDTO
    {
        [StringLength(100, ErrorMessage = "O título pode ter no máximo 100 caracteres.")]
        [PrimeiraLetraMaiuscula(ErrorMessage = "O título deve começar com letra maiúscula.")]
        public string? Titulo { get; set; }

        [StringLength(500, ErrorMessage = "A descrição pode ter no máximo 500 caracteres.")]
        public string? Descricao { get; set; }

        [PrimeiraLetraMaiuscula(ErrorMessage = "O Estado deve começar com letra maiúscula.")]
        public string? Cidade { get; set; }

        public string? Entrada { get; set; } = "Gratuita"; // Entrada padrão como "Gratuita"

        public DateTime? Data { get; set; }

        [StringLength(200, ErrorMessage = "O local pode ter no máximo 200 caracteres.")]
        public string? Local { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O número de vagas deve ser maior que zero.")]
        public int? Vagas { get; set; }
    }
}
