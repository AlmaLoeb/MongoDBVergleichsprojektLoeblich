using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PasswordmanagerApp.Application.Model
{
    [Table("Entry")]
    public class Entry : IEntity<int>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Entry() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // Ean is the PK and not an auto increment column. Annotations are used
        // for the next property (ean)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; private set; }  //primary key
      //  public Guid Guid { get; private set; }
        public virtual string Name { get; }
        [ForeignKey("GroupId")]
        public int GroupId { get; set; }

        //because of self referencing group, this prevents serialisation of navigation property 
        [JsonIgnore]
        public virtual Group Group { get; set; }

    }
}