using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Trade
{
    public class Rights
    {
        [JsonProperty("personal_info")]
        public long PersonalInfo { get; set; }

        [JsonProperty("info")]
        public long Info { get; set; }

        [JsonProperty("trade")]
        public long Trade { get; set; }

        [JsonProperty("withdraw")]
        public long Withdraw { get; set; }
    }
}
