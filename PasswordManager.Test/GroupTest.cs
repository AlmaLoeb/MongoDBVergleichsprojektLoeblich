
using Xunit;

namespace PasswordmanagerApp.Application.Model.Tests
{

    public class GroupTest
    {

        [Fact]
        public void AddEntryTest()
        {
            var group = new Group("name");
            var password = new Password("spengergasse.at", "accountname", "JFKSDHFKJSD", new Pwdpolicies(12, Strongpwd.Middle));
            var bankcard = new Bankcard(12, "Löblich", "Alma", DateTime.Now);
            var idcard = new Idcard(123456, "Löblich", "Alma", new DateTime(2025, 12, 31, 23, 59, 59), new DateTime(2005, 12, 31, 23, 59, 59));
            // Add the entries to the Group
        
            group.AddEntries(bankcard);
            group.AddEntries(idcard);
            group.AddEntries(password);
            // Check if add method worked and entry exists
            Assert.Contains(password, group.Entries);
            Assert.Contains(bankcard, group.Entries);
            Assert.Contains(idcard, group.Entries);
        }
    }
}
