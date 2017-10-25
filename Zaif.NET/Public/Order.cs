using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaifNet.Public
{
    public class Order
    {
        [JsonProperty("amount_rate")]
        public decimal AmountRate { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("total")]
        public decimal Total { get; set; }
    }
}
