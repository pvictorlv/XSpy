using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Base;

namespace XSpy.Database.Entities.Financial
{
    [Table("plans")]
    public class Plan : LazyLoaded
    {
        [Column("id"), Key] public Guid Id { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("append_days")] public int AppendDays { get; set; }
        [Column("price_cents")] public int PriceCents { get; set; }
        [Column("is_active")] public bool IsActive { get; set; }
    }
}