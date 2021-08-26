using Newtonsoft.Json;

namespace XSpy.Database.Models.Data.Financial
{
    public class CreditCard
    {
        [JsonProperty("CardNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string CardNumber { get; set; }

        [JsonProperty("Holder", NullValueHandling = NullValueHandling.Ignore)]
        public string Holder { get; set; }

        [JsonProperty("ExpirationDate", NullValueHandling = NullValueHandling.Ignore)]
        public string ExpirationDate { get; set; }

        [JsonProperty("SecurityCode", NullValueHandling = NullValueHandling.Ignore)]
        public string SecurityCode { get; set; }

        [JsonProperty("Brand", NullValueHandling = NullValueHandling.Ignore)]
        public string Brand { get; set; }

    }
}