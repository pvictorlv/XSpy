using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XSpy.Database.Entities.Roles
{
    [Table("roles")]
    public class Roles
    {
        [Key, Column("name"), MaxLength(50)] public string Name { get; set; }
        [Column("title")] public string Title { get; set; }
    }
}