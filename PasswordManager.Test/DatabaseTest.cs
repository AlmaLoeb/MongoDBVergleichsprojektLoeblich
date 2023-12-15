using PasswordManagerApp.Applcation.Infastruture;

namespace PasswordmanagerApp.Test;

[Collection("Sequential")]
public class DatabaseTest : IDisposable
{
    protected readonly PasswordmanagerContext _db;

    public DatabaseTest()
    {
        _db = PasswordmanagerContext.ForTestConfig();

        _db.Database.EnsureDeleted();
        _db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}