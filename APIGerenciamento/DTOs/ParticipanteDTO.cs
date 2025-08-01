﻿using APIGerenciamento.Models;
using APIGerenciamento.Validations;
using System.ComponentModel.DataAnnotations;

namespace APIGerenciamento.DTOs
{
    public class ParticipanteDTO
    {
       
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        [PrimeiraLetraMaiuscula(ErrorMessage = "O título deve começar com letra maiúscula.")]
        public string? Nome { get; set; }
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "O telefone não é válido.")]
        public string? Telefone { get; set; }

        
    }
}
