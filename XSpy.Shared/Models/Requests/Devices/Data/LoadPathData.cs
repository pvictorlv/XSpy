using System.Collections.Generic;

namespace XSpy.Shared.Models.Requests.Devices
{
    public class LoadPathData
    {
        public string Name { get; set; }
        public string Parent { get; set; }
        public string Path { get; set; }
        public bool CanRead { get; set; }
        public bool IsDir { get; set; }
    }
}