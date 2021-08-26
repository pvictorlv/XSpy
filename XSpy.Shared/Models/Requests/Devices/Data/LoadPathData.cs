namespace XSpy.Database.Models.Requests.Devices.Data
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