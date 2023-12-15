using Bogus.DataSets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordmanagerApp.Application.Model
{
    [Table("IdCard")]
    public class Idcard : Entry
    {
        public Idcard(int idcardnr, string surname, string firstname, DateTime expirationdate, DateTime birthdate) : base()
        {
            Idcardnr = idcardnr;
            Surname = surname;
            Firstname = firstname;
            Expirationdate = expirationdate;
            Birthdate = birthdate;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Idcard() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        //cant use key idcardnr because the id of abstract class entry is already primary key
        //[Key]
        //public int Id { get; private set; }
        public int Idcardnr { get;  set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(50)]
        public string Firstname { get; set; }
        public DateTime Expirationdate { get; set; }
        public DateTime Birthdate { get; set; }

        public override string Name => String.Concat(Firstname, Surname).ToLower();
    }
}