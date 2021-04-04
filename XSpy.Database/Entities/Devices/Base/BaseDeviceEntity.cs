using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XSpy.Database.Entities.Devices.Base
{
    public class BaseDeviceEntity
    {
        [Column("id"), Key] public Guid Id { get; set; }

        [Column("device_id"), ForeignKey(nameof(DeviceData))]
        public Guid DeviceId { get; set; }

        [Column("created_at")] public DateTime CratedAt { get; set; }

        public Device DeviceData { get; set; }
    }
}