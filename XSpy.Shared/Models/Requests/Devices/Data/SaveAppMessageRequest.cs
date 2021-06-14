namespace XSpy.Shared.Models.Requests.Devices
{
    public class SaveAppMessageRequest
    {
        public bool IsOwn { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }
    }
}