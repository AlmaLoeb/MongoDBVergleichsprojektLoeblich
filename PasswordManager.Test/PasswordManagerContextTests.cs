namespace PasswordmanagerApp.Test
{
    public class PasswordManagerContextTests : DatabaseTest
    {
        [Fact]
        public void CreateDatabaseTest()
        {
            //  _db.Database.EnsureCreated();
            _db.Seed();
        }
    }
}