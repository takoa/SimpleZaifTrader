using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace ZaifNet.Trade
{
    public class GetInfo2Response
    {
        [JsonProperty("return")]
        public GetInfo2Return Return { get; set; }

        [JsonProperty("success")]
        public long Success { get; set; }
    }
}
