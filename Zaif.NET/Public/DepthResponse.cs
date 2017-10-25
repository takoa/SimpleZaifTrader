using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Public
{
    public class DepthResponse
    {
        [JsonProperty("asks")]
        public decimal[][] Asks { get; set; }

        [JsonProperty("bids")]
        public decimal[][] Bids { get; set; }
    }
}
