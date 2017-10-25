using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Public
{
    public class StreamingDataLastPrice
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("price_raw")]
        public decimal PriceRaw { get; set; }
    }
}
