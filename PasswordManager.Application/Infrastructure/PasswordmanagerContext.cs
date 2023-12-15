using Bogus;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Application.Model;
using PasswordmanagerApp.Application.Model;
using System.Data;
using System.Diagnostics;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace PasswordManagerApp.Applcation.Infastruture;

public class PasswordmanagerContext : DbContext
{
    public static PasswordmanagerContext ForTestConfig()
    {
        // Docker Container: 
        // docker run -d -p 1433:1433  --name sqlserver2019 -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2019" -latest    
        var opt = new DbContextOptionsBuilder<PasswordmanagerContext>()
            .UseSqlServer(@"Server=127.0.0.1,1433;Initial Catalog=PasswordmanagerTestDB;User Id=sa;Password=SqlServer2019;Encrypt=False")
            .Options;
        return new PasswordmanagerContext(opt);
    }
    // TODO: Add your DbSets
    public DbSet<Bankcard> Bankcards => Set<Bankcard>();
    public DbSet<Idcard> Idcards => Set<Idcard>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Password> Passwords => Set<Password>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Pwdpolicies> PasswordPolicies => Set<Pwdpolicies>();

    public PasswordmanagerContext(DbContextOptions<PasswordmanagerContext> opt) : base(opt) { }

    //code from winter; check if :
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {

            foreach (var key in entityType.GetForeignKeys())
                key.DeleteBehavior = DeleteBehavior.Restrict;

            foreach (var prop in entityType.GetDeclaredProperties())
            {
                // Define Guid as alternate key. The database can create a guid fou you.
                if (prop.Name == "Guid" && entityType.ClrType.BaseType == null)
                {
                    modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                    prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                }

                // Default MaxLength of string Properties is 255.
                if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);

            }
        }

        modelBuilder.Entity<Group>()
            .HasMany(g => g.Entries) // Assuming 'Entries' is the collection navigation property in 'Group'
            .WithOne() // Assuming there is no inverse navigation property in 'Entry'
            .HasForeignKey(e => e.GroupId) // Assuming 'GroupId' is the foreign key in 'Entry'
            .OnDelete(DeleteBehavior.Cascade); // This ensures cascade delete



        modelBuilder
            .Entity<Pwdpolicies>()
            .Property(e => e.Safeness)
            .HasConversion(
                v => v.ToString(),
                v => (Strongpwd)Enum.Parse(typeof(Strongpwd), v));

        modelBuilder
            .Entity<Pwdpolicies>()
            .HasMany(b => b.Passwords)
            .WithOne(a => a.PasswordPolicies)
            .HasForeignKey(a => a.PwdId);

        // i want to delete all entries of a group
        modelBuilder.Entity<Group>()
     .HasMany(g => g.Entries)
     .WithOne(e => e.Group) // Navigation property back to Group
     .HasForeignKey(e => e.GroupId)
     .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Entry>()
            .HasOne(e => e.Group)
            .WithMany(g => g.Entries)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Restrict);



    }


    private void Initialize()
    {
        // Seed logic.
    }

  
    public void Seed()
    {
  
        Randomizer.Seed = new Random(1000);
        var faker = new Faker("de");
        var users = new Faker<User>("de").CustomInstantiator(f =>
        {
            var user = new User(
                    username: f.Person.LastName.ToLower(),
                    initialPassword: "1111" 
                )
            { Guid = f.Random.Guid() };
            return user;
        })
             .Generate(10)
             .GroupBy(a => a.Username).Select(g => g.First())
             .ToList();

        Users.AddRange(users);
        SaveChanges();

        // Add a test user with hashed password
        var testUser = new User("test", "test"); 
        Users.Add(testUser);
        SaveChanges();
        // Generate groups
        var groups = new Faker<Group>("de").CustomInstantiator(f =>
        {
            return new Group(
                name: f.Lorem.Word()
            );
        })
        .Generate(10)
        .ToList();
        Groups.AddRange(groups);
        SaveChanges();

        // Generate bankcards for each group
        foreach (var group in groups)
        {
            var bankcards = new Faker<Bankcard>("de").CustomInstantiator(f =>
            {
                var lastname = f.Name.LastName();
                return new Bankcard(
                    iban: f.Random.Int(10000000, 99999999),
                    surname: lastname,
                    firstname: f.Name.FirstName(),
                    expirationdate: f.Date.Future()
                )
                { GroupId = group.Id };
            })
            .Generate(20)
            .GroupBy(a => a.Iban).Select(g => g.First())
            .ToList();
            Bankcards.AddRange(bankcards);
        }
        SaveChanges();

        var passwordPolicies = new List<Pwdpolicies>();
        for (int length = 8; length <= 20; length++)
        {
            foreach (Strongpwd safeness in Enum.GetValues(typeof(Strongpwd)))
            {
                passwordPolicies.Add(new Pwdpolicies(length, safeness) { Guid = Guid.NewGuid() });
            }
        }
        PasswordPolicies.AddRange(passwordPolicies);
        SaveChanges();




        // Generate passwords for each group
        foreach (var group in groups)
        {
            var passwords = new Faker<Password>("de").CustomInstantiator(f =>
            {
                var passwordLength = f.Random.Int(8, 20); 
                var passwordText = f.Internet.Password(passwordLength);
                var safeness = PasswordHelper.CalculateSafeness(passwordText); 
                var passwordPolicy = passwordPolicies.First(p => p.Length == passwordText.Length && p.Safeness == safeness);
                return new Password(
                    websiteurl: f.Internet.Url(),
                    accountname: f.Internet.UserName(),
                    passworde: passwordText,
                    passwordPolicies: passwordPolicy
                )
                { Guid = f.Random.Guid(), GroupId = group.Id };
            })
            .Generate(20)
            .GroupBy(a => a.Accountname).Select(g => g.First())
            .ToList();

            Passwords.AddRange(passwords);
        }
        SaveChanges();

        // Generate idcards for each group
        foreach (var group in groups)
        {
            var idcards = new Faker<Idcard>("de").CustomInstantiator(f =>
            {

                return new Idcard(
                     idcardnr: f.Random.Int(10000, 99999),
                        surname: f.Name.LastName(),
                        firstname: f.Name.FirstName(),
                        expirationdate: f.Date.Future(),
                        birthdate: f.Date.Past())
                { GroupId = group.Id };
            })
            .Generate(10)
            .GroupBy(a => a.Idcardnr).Select(g => g.First())
            .ToList();
            Idcards.AddRange(idcards);
        }
        SaveChanges();


       

    }
  
    public void CreateDatabase(bool isDevelopment)
    {
        if (isDevelopment) { Database.EnsureDeleted(); }

        if (Database.EnsureCreated()) { Initialize(); }
        if (isDevelopment) Seed();
    }

    public List<Group> FindAllGroups()
    {
        // Use 'Groups' directly, no need for '_relationalContext.'
        return Groups.ToList();
    }

    public List<Group> FindGroupsWithName(string name)
    {
        return Groups.Where(g => g.Name == name).ToList();
    }

    public List<dynamic> FindGroupsWithNameAndProject(string name)
    {
        return Groups
            .Where(g => g.Name == name)
            .Select(g => new
            {
                g.Name,
                Bankcards = g.Entries.OfType<Bankcard>().Select(b => new
                {
                    b.Iban,
                    b.Firstname,
                    b.Surname,
                    b.Expirationdate
                }).ToList()
            }).ToList<dynamic>();
    }

    public List<dynamic> FindGroupsWithNameProjectAndSort(string name)
    {
        return Groups
            .Where(g => g.Name == name)
            .Select(g => new
            {
                g.Name,
                Bankcards = g.Entries.OfType<Bankcard>().Select(b => new
                {
                    b.Iban,
                    b.Firstname,
                    b.Surname,
                    b.Expirationdate
                }).ToList()
            })
            .OrderBy(g => g.Name)
            .ToList<dynamic>();
    }

    public void CreateUsersAndGroups(int numberOfGroups)
    {
        // Create Faker instances for generating random data
        var userFaker = new Faker<User>("de")
            .CustomInstantiator(f => new User(
                username: f.Person.LastName.ToLower(),
                initialPassword: "1111"
            )
            { Guid = f.Random.Guid() })
            .RuleFor(u => u.Username, f => f.Person.UserName)
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password());

        var groupFaker = new Faker<Group>("de")
            .CustomInstantiator(f => new Group(name: f.Commerce.Department()));

        var bankcardFaker = new Faker<Bankcard>("de")
            .CustomInstantiator(f => new Bankcard(
         iban: f.Random.Int(100, 333),
                surname: f.Person.LastName,
                firstname: f.Person.FirstName,
                expirationdate: f.Date.Future())
            );

        var usersToInsert = userFaker.Generate(33);
        Users.AddRange(usersToInsert);

        // Generate and add groups with bankcards
        for (int i = 0; i < numberOfGroups; i++)
        {
            var newGroup = groupFaker.Generate();
            Groups.Add(newGroup);
            SaveChanges(); // Save changes to the database

            var bankcardsToInsert = bankcardFaker.Generate(5);
            foreach (var bankcard in bankcardsToInsert)
            {
                bankcard.GroupId = newGroup.Id;
            }
            Bankcards.AddRange(bankcardsToInsert);
        }

        SaveChanges(); // Save all changes to the database
    }
    public long ReadUsersAndGroups(int count)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var groups = Groups.Include(g => g.Entries).Take(count).ToList();
        // ... additional processing ...

        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    public long UpdateBankcardsFirstnames(string newFirstname, int count)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var bankcardsToUpdate = Bankcards.Take(count).ToList();
        foreach (var bankcard in bankcardsToUpdate)
        {
            bankcard.Firstname = newFirstname;
            Entry(bankcard).State = EntityState.Modified;
        }
        SaveChanges();

        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    public long DeleteGroupsAndEntries(int count)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var groupsToDelete = Groups.Take(count).ToList();
        foreach (var group in groupsToDelete)
        {
            Entry(group).Collection(g => g.Entries).Load();
            Set<Entry>().RemoveRange(group.Entries);
            Groups.Remove(group);
        }
        SaveChanges();

        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    public void ClearRelationalData()
    {
       Bankcards.RemoveRange(Bankcards);
        Passwords.RemoveRange(Passwords);
         SaveChanges();

        var entries = Set<Entry>().ToList();
        Set<Entry>().RemoveRange(entries);
        SaveChanges();

        var groups = Groups.ToList();
        Groups.RemoveRange(groups);
        SaveChanges();

        Users.RemoveRange(Users);
        PasswordPolicies.RemoveRange(PasswordPolicies);
        SaveChanges();
        var userCount = Users.Count();
        var groupCount = Groups.Count();
        var bankcardCount = Bankcards.Count();


        // Print the counts to console
        //Console.WriteLine($"Users remaining: {userCount}");
        //Console.WriteLine($"Groups remaining: {groupCount}");
        //Console.WriteLine($"Bankcards remaining: {bankcardCount}");


    }
}