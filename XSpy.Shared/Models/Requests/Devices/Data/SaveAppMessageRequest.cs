namespace XSpy.Database.Models.Requests.Devices.Data
{
    public class SaveAppMessageRequest
    {
        public bool IsOwn { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }
    }
}