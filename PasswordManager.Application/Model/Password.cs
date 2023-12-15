using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PasswordmanagerApp.Application.Model
{
    [Table("Password")]
    public class Password : Entry
    {
        public Password(string websiteurl, string accountname, string passworde, Pwdpolicies passwordPolicies) : base()
        {
            WebsiteUrl = websiteurl;
            Accountname = accountname;
            Passworde = passworde;
            PasswordPolicies = passwordPolicies;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Password() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.)
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }

        public Guid Guid { get; set; }


        public string Passworde { get; set; }
        public string WebsiteUrl { get; set; }
        public string Accountname { get; set; }

        public int PwdId { get; set; }
        [JsonIgnore]
        [ForeignKey("PwdId")]
        public Pwdpolicies PasswordPolicies { get; set; }

        //  public override string Name => Accountname;

        public void AddPwdPolicy(Pwdpolicies pwd)
        {
            _passwordpolicies.Add(pwd);
        }

        protected List<Pwdpolicies> _passwordpolicies = new();
    }
}