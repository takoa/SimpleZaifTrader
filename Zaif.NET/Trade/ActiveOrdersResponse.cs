using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaifNet.Trade
{
    public class ActiveOrdersResponse
    {
        [JsonProperty("return")]
        public ActiveOrdersReturn[] Return { get; set; }

        [JsonProperty("success")]
        public long Success { get; set; }
    }
}
