using System;
using Newtonsoft.Json;

namespace Stock.Shared.Models.Data
{
    public class UserData
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonIgnore] public string UserHash { get; set; }
    }
}