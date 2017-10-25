using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZaifNet.Public
{
    public class LastPriceResponse
    {
        [JsonProperty("last_price")]
        public decimal Price { get; set; }
    }
}
