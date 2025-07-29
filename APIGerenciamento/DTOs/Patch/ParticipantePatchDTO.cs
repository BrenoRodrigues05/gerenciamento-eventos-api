using APIGerenciamento.Validations;
using System.ComponentModel.DataAnnotations;

namespace APIGerenciamento.DTOs.Patch
{
    public class ParticipantePatchDTO
    {
        [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        [PrimeiraLetraMaiuscula(ErrorMessage = "O nome deve começar com letra maiúscula.")]
        public string? Nome { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "O telefone não é válido.")]
        public string? Telefone { get; set; }
    }
}
