using System;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Base;

namespace XSpy.Database.Entities.Financial
{
    [Table("plans")]
    public class Plan : LazyLoaded
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int? AppendDays { get; set; }
        public int? PriceCents { get; set; }

        public bool? IsActive { get; set; }
    }
}