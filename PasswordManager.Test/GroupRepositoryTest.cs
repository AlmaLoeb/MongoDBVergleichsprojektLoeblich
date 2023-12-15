/*using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PasswordmanagerApp.Application.Infrastructure;
using PasswordmanagerApp.Test;
using Assert = Xunit.Assert;

namespace PasswordmanagerApp.Application.Model.Tests
{
    [TestClass()]
    public class GroupRepositoryTest : DatabaseTest
    {

        private readonly GroupRepository _groupRepository;
        private readonly EntryRepository _entryRepository;

        public GroupRepositoryTest()
        {
            _groupRepository = new GroupRepository(_db);
            _entryRepository = new EntryRepository(_db);
        }

        public static Group GenerateGroup()
        {
            var faker = new Faker<Group>()
                .CustomInstantiator(f =>
                  new Group(
                    name: f.Lorem.Word()));
            return faker.Generate();
        }

        [Fact]
        public void AddGroupTest()
        {
            var group = GenerateGroup();
            _groupRepository.InsertOne(group);
            group.AddEntries(new Idcard(123456, "Löblich", "Alma", new DateTime(2025, 12, 31, 23, 59, 59), new DateTime(2005, 12, 31, 23, 59, 59)));
            Assert.Equal(1, _db.Groups.First().Entries.Count);
            var result = _db.Groups.FirstOrDefault(g => g.Id == group.Id);
            Assert.NotNull(result);
            Assert.Equal(group.Name, result.Name);
        }

    }
}*/