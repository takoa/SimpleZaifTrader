using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaifNet.Public
{
    public class Candle
    {
        [JsonProperty("low")]
        public decimal Low { get; set; }

        [JsonProperty("close")]
        public decimal Close { get; set; }

        [JsonProperty("average")]
        public decimal Average { get; set; }

        [JsonProperty("high")]
        public decimal High { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("open")]
        public decimal Open { get; set; }

        [JsonProperty("volume")]
        public decimal Volume { get; set; }
    }
}
