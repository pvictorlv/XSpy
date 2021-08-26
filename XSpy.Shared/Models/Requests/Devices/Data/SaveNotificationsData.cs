namespace XSpy.Database.Models.Requests.Devices.Data
{
    public class SaveNotificationsData
    {
        public string AppName { get; set; }
        public string Title { get; set; }
        public long PostTime { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }
    }
}