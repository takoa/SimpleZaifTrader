using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Public
{
    public class TradeResponse
    {
        [JsonProperty("currency_pair")]
        public string CurrencyPair { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("tid")]
        public long Tid { get; set; }

        [JsonProperty("trade_type")]
        public string TradeType { get; set; }
    }
}
