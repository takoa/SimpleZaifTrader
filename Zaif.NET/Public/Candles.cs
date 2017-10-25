using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaifNet.Public
{
    public class Candles
    {
        [JsonProperty("1h")]
        public Candle H1 { get; set; }

        [JsonProperty("5m")]
        public Candle M5 { get; set; }

        [JsonProperty("15m")]
        public Candle M15 { get; set; }

        [JsonProperty("12h")]
        public Candle H12 { get; set; }

        [JsonProperty("1d")]
        public Candle D1 { get; set; }

        [JsonProperty("30m")]
        public Candle M30 { get; set; }

        [JsonProperty("1m")]
        public Candle M1 { get; set; }

        [JsonProperty("4h")]
        public Candle H4 { get; set; }

        [JsonProperty("tick")]
        public Candle Tick { get; set; }

        [JsonProperty("8h")]
        public Candle H8 { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }
    }
}
