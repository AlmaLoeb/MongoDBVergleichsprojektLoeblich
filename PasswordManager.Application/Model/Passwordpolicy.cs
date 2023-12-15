using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PasswordmanagerApp.Application.Model
{
    public enum Strongpwd //the enumeration 
    {
        Weak,
        Middle,
        Strong,
    }
    public class Pwdpolicies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PwdId { get; set; }

        public Guid Guid { get; set; }
        public int Length { get; set; }
        public Strongpwd Safeness { get; set; } //still make setable 

        public ICollection<Password> Passwords { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Pwdpolicies(int length, Strongpwd safeness)
        {
            Length = length;
            Safeness = safeness;
        }



        /*  public override bool Equals(object obj)
          {
              return obj is Pwdpolicies password &&
                     Length == password.Length &&
                     Safeness == password.Safeness;
          }*/

        public override int GetHashCode()
        {
            return HashCode.Combine(Length, Safeness);
        }
        /*  //It is at least 12 characters long.
          It contains a mix of uppercase and lowercase letters, numbers, and special characters.
          It does not contain easily guessable information, such as a dictionary word or your name.
          It is not a commonly used password, such as "password" or "1234."
  It is unique to the account for which it is being used, and is not reused across multiple accounts.
          it's not something available publicly.*/
    }

}