using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;
using XSpy.Shared.Models.Interfaces;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_voice_records")]
    public class VoiceRecord : BaseDeviceEntity
    {
        [Column("original_name")] public string OriginalName { get; set; }
        [Column("file_path")] public string SavedPath { get; set; }
     
    }
}