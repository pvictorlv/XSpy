using System;
using Newtonsoft.Json;

namespace Stock.Shared.Models.Data
{
    public class UserData
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string ProfilePhoto { get; set; }
        public Guid? DeviceToken { get; set; }
        public string Document { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        [JsonIgnore] public string UserHash { get; set; }
    }
}