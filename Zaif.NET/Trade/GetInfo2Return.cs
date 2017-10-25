using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Trade
{
    public class GetInfo2Return
    {
        [JsonProperty("funds")]
        public Deposit Funds { get; set; }

        [JsonProperty("rights")]
        public Rights Rights { get; set; }

        [JsonProperty("deposit")]
        public Deposit Deposit { get; set; }

        [JsonProperty("open_orders")]
        public long OpenOrders { get; set; }

        [JsonProperty("server_time")]
        public long ServerTime { get; set; }
    }
}
