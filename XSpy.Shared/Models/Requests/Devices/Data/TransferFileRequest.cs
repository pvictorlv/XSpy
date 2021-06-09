namespace XSpy.Shared.Models.Requests.Devices
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