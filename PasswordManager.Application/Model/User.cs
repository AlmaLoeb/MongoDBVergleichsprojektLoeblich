using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PasswordmanagerApp.Application.Model;

namespace PasswordManager.Application.Model;


public class User : IEntity<int>
{
    public User(
        string username, string initialPassword)
    {
    
        Username = username;
        SetPassword(initialPassword);
    }

#pragma warning disable CS8618
    protected User() { }
#pragma warning restore CS8618
    public Guid Guid { get; set; }
    public int Id { get; private set; }
    public string Username { get; set; }
    public string Salt { get; set; }
    public string PasswordHash { get; set; }

    [MemberNotNull(nameof(Salt), nameof(PasswordHash))]
    public void SetPassword(string password)
    {
        Salt = GenerateRandomSalt();
        PasswordHash = CalculateHash(password, Salt);
    }
    public bool CheckPassword(string password) => PasswordHash == CalculateHash(password, Salt);

    private string GenerateRandomSalt(int length = 128)
    {
        byte[] salt = new byte[length / 8];
        using (System.Security.Cryptography.RandomNumberGenerator rnd =
            System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rnd.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }

    private string CalculateHash(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

        System.Security.Cryptography.HMACSHA256 myHash =
            new System.Security.Cryptography.HMACSHA256(saltBytes);

        byte[] hashedData = myHash.ComputeHash(passwordBytes);

        // Das Bytearray wird als Hexstring zurückgegeben.
        return Convert.ToBase64String(hashedData);
    }
}