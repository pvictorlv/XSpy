using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;


namespace XSpy.Database.Entities.Devices
{
    [Table("device_installed_apps")]
    public class InstalledApps : BaseDeviceEntity
    {
        [Column("app_name"), MaxLength(120)] public string AppName { get; set; }
        [Column("package_name")] public string PackageName { get; set; }
        [Column("version_name"), MaxLength(100)] public string VersionName { get; set; }
        [Column("version_code")] public int VersionCode { get; set; }

    }
}