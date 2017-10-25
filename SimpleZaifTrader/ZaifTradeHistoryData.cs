using CryptoWei;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleZaifTrader
{
    public class ZaifTradeHistoryData : ITradeHistoryData
    {
        public DateTime Date { get; private set; }
        public TradeTypes Type { get; private set; }
        public decimal Price { get; private set; }
        public decimal Amount { get; private set; }

        public ZaifTradeHistoryData()
        {
        }

        public void Update(ZaifNet.Public.Log tradeHistoryData)
        {
            this.Date = DateTime.ParseExact(tradeHistoryData.Timestamp, "HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            this.Type = 0 <= tradeHistoryData.ColoredPrice.IndexOf("text-success") ? TradeTypes.Bid : TradeTypes.Ask;
            this.Price = decimal.Parse(Regex.Replace(tradeHistoryData.ColoredPrice, @"<span class=.*?>(?<price>.*?)</span>", @"${price}"));
            this.Amount = tradeHistoryData.Amount;
        }

        public void Update(ZaifNet.Public.TradeResponse tradeHistoryData)
        {
            this.Date = DateTimeOffset.FromUnixTimeMilliseconds(tradeHistoryData.Date).Date;
            this.Type = tradeHistoryData.TradeType.Equals("bid") ? TradeTypes.Bid : TradeTypes.Ask;
            this.Price = tradeHistoryData.Price;
            this.Amount = tradeHistoryData.Amount;
        }
    }
}
