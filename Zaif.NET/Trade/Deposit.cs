using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Trade
{
    public class Deposit
    {
        [JsonProperty("jpy")]
        public long Jpy { get; set; }

        [JsonProperty("btc")]
        public decimal Btc { get; set; }

        [JsonProperty("xem")]
        public decimal Xem { get; set; }

        [JsonProperty("mona")]
        public long Mona { get; set; }

        [JsonProperty("bch")]
        public decimal Bch { get; set; }

        [JsonProperty("eth")]
        public decimal Eth { get; set; }

        [JsonProperty("zaif")]
        public decimal Zaif { get; set; }

        [JsonProperty("xcp")]
        public decimal Xcp { get; set; }

        [JsonProperty("bitcrystals")]
        public decimal Bcy { get; set; }

        [JsonProperty("sjcx")]
        public decimal Sjcx { get; set; }

        [JsonProperty("fscc")]
        public decimal Fscc { get; set; }

        [JsonProperty("pepecash")]
        public decimal Pepecash { get; set; }

        [JsonProperty("cicc")]
        public decimal Cicc { get; set; }

        [JsonProperty("ncxc")]
        public decimal Ncxc { get; set; }
    }
}