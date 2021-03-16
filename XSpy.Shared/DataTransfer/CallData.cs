using System;
using System.Text.Json.Serialization;
using XSpy.Database.XSpy.Shared;
using XSpy.Database.XSpy.Shared.Models.Interfaces;
using XSpy.Shared.Models.Interfaces;

namespace XSpy.Shared.DataTransfer
{
    public class CallData : ICallEntity
    {
        public Guid? Id { get; set; }
        [JsonPropertyName("phoneNo")]public string Number { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("duration")] public string Duration { get; set; }
        [JsonPropertyName("date")] public string Date { get; set; }
        [JsonPropertyName("type")] public CallType Type { get; set; }
    }
}