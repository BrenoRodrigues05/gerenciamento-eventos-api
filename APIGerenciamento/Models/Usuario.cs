using APIGerenciamento.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace APIGerenciamento.Models
{
    public class Usuario : IEntidade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string SenhaHash { get; set; } = string.Empty; // Senha criptografada

        [Required]
        public string Role { get; set; } = "User" ; // "Admin", "User", etc.
    }
}
