using System;

namespace CFCEad.Shared.Models.Response.Devices.Search
{
    public class DeviceListResponse
    {
        public Guid Id { get; set; }
        public string DeviceId { get; set; }
        public string Model { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}