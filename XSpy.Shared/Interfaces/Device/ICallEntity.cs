using System;
using XSpy.Database.XSpy.Shared.Models.Interfaces;

namespace XSpy.Shared.Models.Interfaces
{
    public interface ICallEntity
    {
        public Guid? Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public string Date { get; set; }
        public CallType Type { get; set; }
        public IDeviceEntity DeviceData { get; set; }
    }
}