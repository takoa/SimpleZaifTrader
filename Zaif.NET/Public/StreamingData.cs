using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Public
{
    public class StreamingData
    {
        [JsonProperty("currency_pair")]
        public string CurrencyPair { get; set; }

        [JsonProperty("bid_data")]
        public Order[] Bids { get; set; }

        [JsonProperty("ask_data")]
        public Order[] Asks { get; set; }

        [JsonProperty("candles")]
        public Candles Candles { get; set; }

        [JsonProperty("log_data")]
        public Log[] Logs { get; set; }

        [JsonProperty("last_price")]
        public StreamingDataLastPrice LastPrice { get; set; }

        [JsonProperty("target_users")]
        public string[] TargetUsers { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }
}
