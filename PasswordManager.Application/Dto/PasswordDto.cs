using Microsoft.Extensions.DependencyInjection;
using PasswordmanagerApp.Application.Model;
using PasswordManagerApp.Applcation.Infastruture;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PasswordmanagerApp.Application.Dto
{
    public record PasswordForPostDTO(
     Guid Guid,
     int GroupId,

     [Url(ErrorMessage = "Ungültige URL.")]
    [StringLength(2048, MinimumLength = 7, ErrorMessage = "Die Länge des URL ist ungültig.")]
    string WebsiteUrl,

     string Accountname,

     [StringLength(2048, MinimumLength = 8, ErrorMessage = "Die Länge des Passwords ist ungültig.")]
    string Passworde,
      Guid PasswordPoliciesGuid
 ) : IValidatableObject
    
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            //  Accountname
            if (string.IsNullOrWhiteSpace(Accountname))
            {
                yield return new ValidationResult("Accountname is required.", new[] { nameof(Accountname) });
            }
            else if (Accountname.Length < 3 || Accountname.Length > 100)
            {
                yield return new ValidationResult("Accountname must be between 3 and 100 characters.", new[] { nameof(Accountname) });
            }

            //  Password
            if (string.IsNullOrWhiteSpace(Passworde))
            {
                yield return new ValidationResult("Password is required.", new[] { nameof(Passworde) });
            }
            else if (Passworde.Length < 8)
            {
                yield return new ValidationResult("Password must be at least 8 characters.", new[] { nameof(Passworde) });
            }
            else
            {
                var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
                if (!passwordRegex.IsMatch(Passworde))
                {
                    yield return new ValidationResult("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.", new[] { nameof(Passworde) });
                }
            }
            var db = validationContext.GetRequiredService<PasswordmanagerContext>();

            //  PasswordPoliciesGuid
            if (PasswordPoliciesGuid == Guid.Empty)
             {
                 yield return new ValidationResult("PasswordPoliciesGuid is required.", new[] { nameof(PasswordPoliciesGuid) });
             }

             if (!db.PasswordPolicies.Any(a => a.Guid == PasswordPoliciesGuid))
             {
                 yield return new ValidationResult("Passwordpolicy does not exist", new[] { nameof(PasswordPoliciesGuid) });
             }

            if (!db.Groups.Any(g => g.Id == GroupId))
            {
                yield return new ValidationResult("Die angegebene Gruppe existiert nicht.", new[] { nameof(GroupId) });
            }
        }
    }
    public record PasswordForPutDTO(
     Guid Guid,
     [Url(ErrorMessage = "Ungültige URL.")]
    [StringLength(2048, MinimumLength = 7, ErrorMessage = "Die Länge des URL ist ungültig.")]
    string WebsiteUrl,

     string Accountname,

     [StringLength(2048, MinimumLength = 8, ErrorMessage = "Die Länge des Passwords ist ungültig.")]
    string Passworde,
      Guid PasswordPoliciesGuid
 ) : IValidatableObject

    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            //  Accountname
            if (string.IsNullOrWhiteSpace(Accountname))
            {
                yield return new ValidationResult("Accountname is required.", new[] { nameof(Accountname) });
            }
            else if (Accountname.Length < 3 || Accountname.Length > 100)
            {
                yield return new ValidationResult("Accountname must be between 3 and 100 characters.", new[] { nameof(Accountname) });
            }

            //  Password
            if (string.IsNullOrWhiteSpace(Passworde))
            {
                yield return new ValidationResult("Password is required.", new[] { nameof(Passworde) });
            }
            else if (Passworde.Length < 8)
            {
                yield return new ValidationResult("Password must be at least 8 characters.", new[] { nameof(Passworde) });
            }
            else
            {
                var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
                if (!passwordRegex.IsMatch(Passworde))
                {
                    yield return new ValidationResult("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.", new[] { nameof(Passworde) });
                }
            }
            var db = validationContext.GetRequiredService<PasswordmanagerContext>();

            //  PasswordPoliciesGuid
            if (PasswordPoliciesGuid == Guid.Empty)
            {
                yield return new ValidationResult("PasswordPoliciesGuid is required.", new[] { nameof(PasswordPoliciesGuid) });
            }

            if (!db.PasswordPolicies.Any(a => a.Guid == PasswordPoliciesGuid))
            {
                yield return new ValidationResult("Passwordpolicy does not exist", new[] { nameof(PasswordPoliciesGuid) });
            }
        }
    }

}