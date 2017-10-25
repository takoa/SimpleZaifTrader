using Newtonsoft.Json;

namespace ZaifNet.Public
{
    public class Log
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("colored_price")]
        public string ColoredPrice { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }
}