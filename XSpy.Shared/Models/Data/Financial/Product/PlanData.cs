using System;

namespace XSpy.Database.Models.Data.Financial.Product
{
    public class PlanData
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int? AppendDays { get; set; }
        public int? PriceCents { get; set; }

        public bool? IsActive { get; set; }
    }
}