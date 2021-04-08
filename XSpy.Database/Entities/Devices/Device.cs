using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using XSpy.Database.Entities.Base;

namespace XSpy.Database.Entities.Devices
{
    [Table("devices")]
    public class Device : LazyLoaded
    {
        public Device(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public Device()
        {
        }

        private User _userData;
        private ICollection<Contact> _contacts;
        private ICollection<Call> _calls;
        [Key, Column("id")] public Guid Id { get; set; }

        [Column("user_id"), ForeignKey(nameof(UserData))]
        public Guid UserId { get; set; }

        [Column("device_id"), MaxLength(50)] public string DeviceId { get; set; }
        [Column("model"), MaxLength(40)] public string Model { get; set; }

        [Column("manufacturer"), MaxLength(40)]
        public string Manufacturer { get; set; }

        [Column("sys_version"), MaxLength(30)] public string SystemVersion { get; set; }
        [Column("last_ip"), MaxLength(45)] public string LastIp { get; set; }
        [Column("is_online")] public bool IsOnline { get; set; }

        [Column("added_at")] public DateTime AddedAt { get; set; }
        [Column("updated_at")] public DateTime UpdatedAt { get; set; }

        public User UserData
        {
            get => LazyLoader.Load(this, ref _userData);
            set => _userData = value;
        }


        public ICollection<Contact> Contacts
        {
            get => LazyLoader.Load(this, ref _contacts);
            set => _contacts = value;
        }

        public ICollection<Call> Calls
        {
            get => LazyLoader.Load(this, ref _calls);
            set => _calls = value;
        }
    }
}