using System.ComponentModel.DataAnnotations;

namespace APIGerenciamento.Validations
{
    public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var texto = value.ToString()!;
            var primeiraLetra = texto[0].ToString();

            if (primeiraLetra == primeiraLetra.ToUpper())
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("A primeira letra deve ser maiúscula.");
        }
    }
}
