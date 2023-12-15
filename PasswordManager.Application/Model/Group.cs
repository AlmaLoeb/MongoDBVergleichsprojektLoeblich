using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PasswordmanagerApp.Application.Model
{
    [Table("Group")]
    public class Group : IEntity<int>
    {
        public Group(string name)
        {
            Name = name;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Group() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Key]
        public int Id { get; private set; }  //primary key
        [MaxLength(50)]
        public string Name { get; set; }

        protected List<Entry> _entries = new();
        // Add JsonIgnore to prevent serialization of Entries
        [JsonIgnore]
        public virtual IReadOnlyCollection<Entry> Entries => _entries;

        public void AddEntries(Entry entry)
        {
            _entries.Add(entry);
        }
    }
}
