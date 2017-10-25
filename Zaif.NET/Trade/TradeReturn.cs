using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Trade
{
    public class TradeReturn
    {
        [JsonProperty("order_id")]
        public long OrderId { get; set; }

        [JsonProperty("funds")]
        public Deposit Funds { get; set; }

        [JsonProperty("received")]
        public decimal Received { get; set; }

        [JsonProperty("remains")]
        public long Remains { get; set; }
    }
}
