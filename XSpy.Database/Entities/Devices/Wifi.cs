using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XSpy.Database.Entities.Devices.Base;

namespace XSpy.Database.Entities.Devices
{
    [Table("device_wifi")]

    public class Wifi : BaseDeviceEntity
    {
        [Column("bssid")]public string Bssid { get; set; }
        [Column("ssid")]public string Ssid { get; set; }
    }
}