using Bogus;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace PasswordManager.Mongo
{

    public class PasswordManagerMongoContext
    {
        public MongoClient Client { get; private set; }
        public IMongoDatabase Db { get; private set; }

        public IMongoCollection<UserDocument> Users { get; private set; }
        public IMongoCollection<GroupDocument> Groups { get; private set; }
        public IMongoCollection<BankcardDocument> Bankcards { get; private set; }

        public static PasswordManagerMongoContext FromConnectionString(string connectionString, bool logging = false)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);

            //if (logging)
            //{
            //    settings.ClusterConfigurator = cb =>
            //    {
            //        cb.Subscribe<CommandStartedEvent>(e =>
            //        {
            //            Console.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
            //        });
            //    };
            //}

            var client = new MongoClient(settings);
            var db = client.GetDatabase("PasswordManagerDb");

            return new PasswordManagerMongoContext(client, db);
        }

        private PasswordManagerMongoContext(MongoClient client, IMongoDatabase db)
        {
            Client = client;
            Db = db;

            Users = Db.GetCollection<UserDocument>("Users");
            Groups = Db.GetCollection<GroupDocument>("Groups");
            Bankcards = Db.GetCollection<BankcardDocument>("Bankcards");
        }

        public void Seed()
        {
            // Drop existing collections 
            Db.DropCollection("Users");
            Db.DropCollection("Groups");

            var faker = new Faker("en");

            // Seed Users
            var userFaker = new Faker<UserDocument>()
                .CustomInstantiator(f => new UserDocument(
                    ObjectId.GenerateNewId(),
                    f.Internet.UserName(),
                    f.Random.Hash(),
                    f.Random.Hash()
                ));

            var usersToInsert = userFaker.Generate(50); // Generate 50 users
            Users.InsertMany(usersToInsert);

            // Seed Groups and Bankcards
            var groupsToInsert = new Faker<GroupDocument>()
                .RuleFor(g => g.Id, f => ObjectId.GenerateNewId())
                .RuleFor(g => g.Name, f => f.Commerce.Department())
                .RuleFor(g => g.Bankcards, f =>
                {
                    var bankcardsList = new List<BankcardDocument>();
                    for (int i = 0; i < f.Random.Int(2, 8); i++)
                    {
                        bankcardsList.Add(new BankcardDocument(
                            Id: ObjectId.GenerateNewId(),
                            Iban: f.Finance.Iban(),
                            Firstname: f.Name.FirstName(),
                            Surname: f.Name.LastName(),
                            ExpirationDate: f.Date.Future()
                        ));
                    }
                    return bankcardsList;
                })
                .Generate(10); // Generate 10 groups with embedded bank cards

            Groups.InsertMany(groupsToInsert);
        }



        public record UserDocument(
            ObjectId Id,
            string Username,
            string Salt,
            string PasswordHash
        );

        public record GroupDocument(
            ObjectId Id = default, 
            string Name = default, 
            List<BankcardDocument> Bankcards = null 
        )
        {

            public GroupDocument() : this(default, default, new List<BankcardDocument>())
            {

            }
        };


        public record BankcardDocument(
         ObjectId Id,
         string Iban,
         string Firstname,
         string Surname,
         DateTime ExpirationDate
     )
        {
            public BankcardDocument(
                string Iban,
                string Firstname,
                string Surname,
                DateTime ExpirationDate
            ) : this(ObjectId.GenerateNewId(), Iban, Firstname, Surname, ExpirationDate)
            {
            }
        }

        // CRUD operations, etc.
        //create
        public void CreateGroup(GroupDocument newGroup)
        {
            Groups.InsertOne(newGroup);
        }
        public void AddBankcardToGroup(ObjectId groupId, BankcardDocument newBankcard)
        {
            var update = Builders<GroupDocument>.Update.Push(g => g.Bankcards, newBankcard);
            Groups.UpdateOne(g => g.Id == groupId, update);
        }
        // read
        public List<GroupDocument> GetAllGroups()
        {
            return Groups.Find(_ => true).ToList();
        }

        public GroupDocument GetGroupById(ObjectId id)
        {
            return Groups.Find(group => group.Id == id).FirstOrDefault();
        }

        public List<BankcardDocument> GetBankcardsByGroupId(ObjectId groupId)
        {
            var group = Groups.Find(g => g.Id == groupId).FirstOrDefault();
            return group?.Bankcards ?? new List<BankcardDocument>();
        }
        public List<UserDocument> GetAllUsers()
        {
            return Users.Find(_ => true).ToList();
        }

        public UserDocument GetUserById(ObjectId id)
        {
            return Users.Find(user => user.Id == id).FirstOrDefault();
        }

        //update
        public void UpdateGroup(ObjectId groupId, GroupDocument updatedGroup)
        {
            Groups.ReplaceOne(group => group.Id == groupId, updatedGroup);
        }

        public void UpdateBankcard(ObjectId groupId, ObjectId bankcardId, BankcardDocument updatedBankcard)
        {
            var update = Builders<GroupDocument>.Update.Set(g => g.Bankcards[-1], updatedBankcard);
            Groups.UpdateOne(g => g.Id == groupId && g.Bankcards.Any(b => b.Id == bankcardId), update);
        }
        public void UpdateUser(ObjectId id, UserDocument updatedUser)
        {
            Users.ReplaceOne(user => user.Id == id, updatedUser);
        }

        //delete
        public void DeleteGroup(ObjectId groupId)
        {
            Groups.DeleteOne(group => group.Id == groupId);
        }

        public void DeleteBankcard(ObjectId groupId, ObjectId bankcardId)
        {
            var update = Builders<GroupDocument>.Update.PullFilter(g => g.Bankcards, b => b.Id == bankcardId);
            Groups.UpdateOne(g => g.Id == groupId, update);
        }
        public void DeleteUser(ObjectId id)
        {
            Users.DeleteOne(user => user.Id == id);
        }

        public long CreateGroups(int count)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var groupsToInsert = new Faker<GroupDocument>()
                .RuleFor(g => g.Id, f => ObjectId.GenerateNewId())
                .RuleFor(g => g.Name, f => f.Commerce.Department())
                .RuleFor(g => g.Bankcards, f => Enumerable.Range(0, f.Random.Int(2, 8)).Select(_ => new BankcardDocument(
                    ObjectId.GenerateNewId(),
                    f.Finance.Iban(),
                    f.Name.FirstName(),
                    f.Name.LastName(),
                    f.Date.Future()))
                .ToList())
                .Generate(count);

            Groups.InsertMany(groupsToInsert);

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        public long ReadGroups(int count)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var groups = Groups.Find(_ => true).Limit(count).ToList();
            foreach (var group in groups)
            {
                var bankcards = group.Bankcards; 
            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        public long UpdateGroups(int count)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var updatedName = "UpdatedGroupName";
            var filter = Builders<GroupDocument>.Filter.Empty; // Filter to select all documents
            var update = Builders<GroupDocument>.Update.Set(g => g.Name, updatedName);

            // Update all documents
            Groups.UpdateMany(filter, update);

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        public long DeleteGroups(int count)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var groupsToDelete = Groups.Find(_ => true).Limit(count).ToList();
            foreach (var group in groupsToDelete)
            {
                Groups.DeleteOne(g => g.Id == group.Id);
            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        public List<BsonDocument> FindGroupsWithNameAndProject(string name)
        {
            // Define the projection to include only specific fields
            var projection = Builders<GroupDocument>.Projection
                .Include(g => g.Name)
                .Include(g => g.Bankcards); // The Bankcards field itself is included, not its subfields

            // Execute the query with the specified projection
            return Groups.Find(g => g.Name == name)
                         .Project<BsonDocument>(projection)
                         .ToList();
        }

        public List<BsonDocument> FindGroupsWithNameProjectAndSort(string name)
        {
            // Define the projection 
            var projection = Builders<GroupDocument>.Projection
                .Include(g => g.Name)
                .Include(g => g.Bankcards); 

            // Define the sorting criteria
            var sort = Builders<GroupDocument>.Sort.Ascending(g => g.Name);

            // Execute the query with projection and sorting
            return Groups.Find(g => g.Name == name)
                         .Project<BsonDocument>(projection)
                         .Sort(sort)
                         .ToList();
        }


        public List<GroupDocument> FindGroupsWithName(string name)
        {
            return Groups.Find(g => g.Name == name).ToList();
        }

        public long UpdateBankcardsFirstname(string newFirstname, int count)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var filter = Builders<GroupDocument>.Filter.Empty;
            var update = Builders<GroupDocument>.Update.Set("Bankcards.$[].Firstname", newFirstname);

            Groups.UpdateMany(filter, update);

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }


    }
}
