namespace XSpy.Database.Models.Requests.Devices.Data
{
    public class TransferFileRequest
    {
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public byte[] Buffer { get; set; }
    }
}