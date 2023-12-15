using System;
using System.Diagnostics;
using Bogus;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using PasswordmanagerApp.Application.Model;
using PasswordManagerApp.Applcation.Infastruture;

namespace PasswordManager.Mongo
{
    public class CrudPerformanceTester
    {

        private PasswordmanagerContext _relationalContext;
        private PasswordManagerMongoContext _mongoContext;

        public CrudPerformanceTester()
        {
            // Setup relational context
            var relationalOptions = new DbContextOptionsBuilder<PasswordmanagerContext>()
                .UseSqlServer(@"Server=127.0.0.1,1433;Initial Catalog=PasswordmanagerTestDB;User Id=sa;Password=SqlServer2019;Encrypt=False")
                .Options;
            _relationalContext = new PasswordmanagerContext(relationalOptions);

            // Setup MongoDB context
            _mongoContext = PasswordManagerMongoContext.FromConnectionString("mongodb://localhost:27017", logging: true);

            // Ensure databases are created and seeded
            _relationalContext.Database.EnsureCreated();
            _relationalContext.Seed();
            _mongoContext.Seed();
        }

        public void RunTests()
        {
            int[] recordCounts = new int[] { 100, 1000, 10000 };

            foreach (int count in recordCounts)
            {
                // Clear existing data and seed
                _relationalContext.ClearRelationalData();
                _relationalContext.Seed();

                // Perform operations and measure time
                long relationalCreateTime = PerformCreateOperation(_relationalContext, count);
                long mongoCreateTime = _mongoContext.CreateGroups(count);

                long relationalFindTime1 = MeasureRelationalFindOperation<Group>(() => _relationalContext.FindAllGroups());
                long relationalFindTime2 = MeasureRelationalFindOperation<Group>(() => _relationalContext.FindGroupsWithName("Kids"));
                long relationalFindTime3 = MeasureRelationalFindOperation<dynamic>(() => _relationalContext.FindGroupsWithNameAndProject("Kids"));
                long relationalFindTime4 = MeasureRelationalFindOperation<dynamic>(() => _relationalContext.FindGroupsWithNameProjectAndSort("Kids"));


                // Sum up the times of the four relational find operations
                long relationalReadTime = relationalFindTime1 + relationalFindTime2 + relationalFindTime3 + relationalFindTime4;

                long mongoFindTime1 = MeasureMongoFindOperation(() => _mongoContext.GetAllGroups());
                long mongoFindTime2 = MeasureMongoFindOperation(() => _mongoContext.FindGroupsWithName("Kids"));
                long mongoFindTime3 = MeasureMongoFindOperation(() => _mongoContext.FindGroupsWithNameAndProject("Kids").ToList());
                long mongoFindTime4 = MeasureMongoFindOperation(() => _mongoContext.FindGroupsWithNameProjectAndSort("Kids").ToList());

                // Sum up the times of the four find operations
                long mongoReadTime = mongoFindTime1 + mongoFindTime2 + mongoFindTime3 + mongoFindTime4;


                // Update test
                string newFirstname = "NewFirstName";
                long relationalUpdateDuration = _relationalContext.UpdateBankcardsFirstnames(newFirstname, count);
                long mongoUpdateDuration = _mongoContext.UpdateBankcardsFirstname(newFirstname, count);



                long relationalDeleteTime = _relationalContext.DeleteGroupsAndEntries(count);
                long mongoDeleteTime = _mongoContext.DeleteGroups(count);

                // Print Comparison Table for each record count
                Console.WriteLine($"Performance Comparison for {count} Records:");
                Console.WriteLine("Operation\tRelational DB Time (ms)\tMongoDB Time (ms)");
                Console.WriteLine($"Create\t\t{relationalCreateTime}\t\t{mongoCreateTime}");
                Console.WriteLine($"Read\t\t{relationalReadTime}\t\t{mongoReadTime}");
                Console.WriteLine($"Update\t\t{relationalUpdateDuration}\t\t{mongoUpdateDuration}");
                Console.WriteLine($"Delete\t\t{relationalDeleteTime}\t\t{mongoDeleteTime}");
                Console.WriteLine("-------------------------------------------------");
            }
        }


        private long PerformCreateOperation(PasswordmanagerContext context, int numberOfGroups)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            context.CreateUsersAndGroups(numberOfGroups);

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        private long MeasureRelationalFindOperation<T>(Func<List<T>> findOperation)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            findOperation.Invoke();

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private long MeasureMongoFindOperation<T>(Func<List<T>> findOperation)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Perform the find operation
            findOperation.Invoke();

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

    }
}
