using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared;


namespace XSpy.Database.Entities.Devices
{
    [Table("device_file_list")]
    public class FileList : BaseDeviceEntity
    {
        [Column("original_path")] public string OriginalPath { get; set; }
        [Column("thumb_path")] public string ThumbPath { get; set; }
        [Column("file_id")] public int FileId { get; set; }
    }
}