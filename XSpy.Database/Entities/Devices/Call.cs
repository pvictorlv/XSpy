using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;
using XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Devices
{
    [Table("calls")]
    public class Call : ICallEntity
    {
        [Column("id"), Key] public Guid? Id { get; set; }
        [Column("number"), MaxLength(30)] public string Number { get; set; }
        [Column("number"), MaxLength(255)] public string Name { get; set; }
        [Column("number"), MaxLength(10)] public string Duration { get; set; }
        [Column("number"), MaxLength(30)] public string Date { get; set; }
        [Column("call_type")] public CallType Type { get; set; }

        public IDeviceEntity DeviceData { get; set; }
    }
}