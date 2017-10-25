using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Trade
{
    public class TradeResponse
    {
        [JsonProperty("return")]
        public TradeReturn Return { get; set; }

        [JsonProperty("success")]
        public long Success { get; set; }
    }
}
