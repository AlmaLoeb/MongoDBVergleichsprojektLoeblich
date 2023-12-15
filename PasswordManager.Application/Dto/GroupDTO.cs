/*using PasswordmanagerApp.Application.Dto;
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
  
        public record GroupDTO(
          int Id,
          [StringLength( maximumLength: 50, ErrorMessage = "Die Länge des Namens ist ungültig.")]
          string Name,
          List<PasswordDto> Entry,
          List<Guid> EntryGuids
          ) : IValidatableObject
        {
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {

                //  Accountname
                if (string.IsNullOrWhiteSpace(Name))
                {
                    yield return new ValidationResult("Name for group is required.", new[] { nameof(Name) });
                }
                else if ( Name.Length > 50)
                {
                    yield return new ValidationResult("Name for group can not exceed 50 characters.", new[] { nameof(Name) });
                }


              foreach (EntryDTO var in Entry)
        {
            if (!db.ScanResultRepository.Queryable.Where(d => d.Guid == var.Guid).Any())
                yield return new ValidationResult("ScanResult Guid does not exist.", new[] {nameof(var)});
        }
            }

        }
    }
}*/
