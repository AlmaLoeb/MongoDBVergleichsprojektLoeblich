using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PasswordmanagerApp.Application.Dto
{
    public record BankcardDto(
      int Iban,
       [StringLength(50, ErrorMessage = "Firstname must not exceed 50 characters.")]
      string Firstname,

      string Surname,
       DateTime Expirationdate
    )
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Firstname))
            {
                yield return new ValidationResult("Firstname is required.", new[] { nameof(Firstname) });
            }

            if (string.IsNullOrWhiteSpace(Surname))
            {
                yield return new ValidationResult("Surname is required.", new[] { nameof(Surname) });
            }

            if (Iban <= 0)
            {
                yield return new ValidationResult("Iban is required.", new[] { nameof(Iban) });
            }

            if (Expirationdate < DateTime.Now)
            {
                yield return new ValidationResult("The card is expired.", new[] { nameof(Expirationdate) });
            }
        }
    }
}