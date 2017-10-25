using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Trade
{
    public class Return
    {
        [JsonProperty("funds")]
        public Deposit Funds { get; set; }

        [JsonProperty("order_id")]
        public long OrderId { get; set; }
    }
}
