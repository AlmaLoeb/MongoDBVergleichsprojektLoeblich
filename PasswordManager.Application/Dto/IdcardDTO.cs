using Bogus.DataSets;
using Microsoft.Extensions.DependencyInjection;
using PasswordManagerApp.Applcation.Infastruture;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PasswordManager.Application.Dto
{
    /* public record IdcardDTO(
         int Idcardnr,
              int GroupId,
        string Firstname,
 string Surname,
 DateTime Expirationdate,
 DateTime Birthdate
        ) : IValidatableObject
     {
         public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
         {
             //  Firstname
             if (string.IsNullOrWhiteSpace(Firstname))
             {
                 yield return new ValidationResult("Firstname is required.", new[] { nameof(Firstname) });
             }
             //  Surname
             if (string.IsNullOrWhiteSpace(Surname))
             {
                 yield return new ValidationResult("Surname is required.", new[] { nameof(Surname) });
             }
             var db = validationContext.GetRequiredService<PasswordmanagerContext>();

             /*  if (GroupId.HasValue && !db.Groups.Any(g => g.Id == GroupId.Value))
               {
                   yield return new ValidationResult("Die angegebene Gruppe existiert nicht.", new[] { nameof(GroupId) });
               }*/
    /* if (!db.Groups.Any(g => g.Id == GroupId))
     {
         yield return new ValidationResult("Die angegebene Gruppe existiert nicht.", new[] { nameof(GroupId) });
     }
 }


}*/
    public record IdcardForPostDTO(
 int GroupId,
 int Idcardnr,
 string Firstname,
 string Surname,
 DateTime Expirationdate,
 DateTime Birthdate
) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
         
            if (Idcardnr <= 0)
            {
                yield return new ValidationResult("Idcardnr is required", new[] { nameof(Idcardnr) });
            }

    
            if (string.IsNullOrWhiteSpace(Firstname))
            {
                yield return new ValidationResult("Firstname is required.", new[] { nameof(Firstname) });
            }
           
            if (string.IsNullOrWhiteSpace(Surname))
            {
                yield return new ValidationResult("Surname is required.", new[] { nameof(Surname) });
            }

            var db = validationContext.GetRequiredService<PasswordmanagerContext>();
            if (!db.Groups.Any(g => g.Id == GroupId))
            {
                yield return new ValidationResult("Die angegebene Gruppe existiert nicht.", new[] { nameof(GroupId) });
            }
        }
    }


    public record IdcardForPutDTO(
    int Idcardnr,
    string Firstname,
    string Surname,
    DateTime Expirationdate,
    DateTime Birthdate
) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //  Firstname
            if (string.IsNullOrWhiteSpace(Firstname))
            {
                yield return new ValidationResult("Firstname is required.", new[] { nameof(Firstname) });
            }
            //  Surname
            if (string.IsNullOrWhiteSpace(Surname))
            {
                yield return new ValidationResult("Surname is required.", new[] { nameof(Surname) });
            }
        }
    }

}