using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;
using XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_permissions")]
    public class Permission : BaseDeviceEntity
    {
        [Column("key"), MaxLength(120)] public string Key { get; set; }
        [Column("isAllowed")] public bool IsAllowed { get; set; }

    }
}