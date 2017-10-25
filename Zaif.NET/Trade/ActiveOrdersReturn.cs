using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaifNet.Trade
{
    public class ActiveOrdersReturn
    {
        [JsonProperty("currency_pair")]
        public string CurrencyPair { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("id")]
        public long ID { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        public override bool Equals(object obj)
        {
            ActiveOrdersReturn aor = obj as ActiveOrdersReturn;

            if (aor == null)
            {
                return false;
            }

            return this.CurrencyPair.Equals(aor.CurrencyPair)
                && this.ID == aor.ID
                && this.Action.Equals(aor.Action)
                && this.Price == aor.Price
                && this.Amount == aor.Amount
                && this.Timestamp.Equals(aor.Timestamp)
                && this.Comment.Equals(aor.Comment);
        }
    }
}
