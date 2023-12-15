using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordmanagerApp.Application.Model
{
    [Table("Bankcard")]
    public class Bankcard : Entry
    {
        public Bankcard(int iban, string surname, string firstname, DateTime expirationdate) : base()
        {
            Iban = iban;
            Surname = surname;
            Firstname = firstname;
            Expirationdate = expirationdate;

        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Bankcard() { }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

     
        public int Iban { get; private set; }
        [MaxLength(50)]
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public DateTime Expirationdate { get; set; }

        public override string Name => String.Concat(Firstname, Surname).ToLower();
    }
}