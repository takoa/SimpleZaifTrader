using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Trade
{
    public class CancelOrderResponse
    {
        [JsonProperty("return")]
        public Return Return { get; set; }

        [JsonProperty("success")]
        public long Success { get; set; }
    }
}
