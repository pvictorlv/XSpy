using System;

namespace XSpy.Database.XSpy.Shared.Models.Interfaces
{
    public interface IDeviceEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string LastIp { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}