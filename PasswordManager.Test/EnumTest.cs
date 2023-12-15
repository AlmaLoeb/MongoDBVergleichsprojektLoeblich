using PasswordmanagerApp.Application.Model;

namespace PasswordmanagerApp.Test;

public class EnumTest : DatabaseTest

{
    [Fact]
    public static Pwdpolicies GeneratePwdPolicies()
    {
        var pwdpol = new Pwdpolicies(

                length: 10,
           safeness: Strongpwd.Middle);
        return pwdpol;
    }
    public record Passwordpolicy(int Length, Strongpwd Safeness);


    public static Password AddPasswordTest(Application.Model.Pwdpolicies passwordPolicies)
    {
        var pwd = new Password(
           websiteurl: "htlspengergasse.com",
           accountname: "lisa43",
           passworde: "lisa0505",
      passwordPolicies: passwordPolicies);
        return pwd;
    }

    /*  
   [Fact]
public void ValueConversionSuccessTest()
   {
       var pwd = AddPasswordTest();
       var pwdpolicies = GeneratePwdPolicies();
       //  var user = AddUserTest();
       pwd.AddPwdPolicy(pwdpolicies);
       _db.Passwords.Add(pwd);
       _db.SaveChanges();
       Assert.Equal(Strongpwd.Middle, _db.Passwords.First().PasswordPolicye);
   }*/

}
