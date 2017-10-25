using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWei
{
    public interface ITradeHistoryData
    {
        DateTime Date { get; }
        TradeTypes Type { get; }
        decimal Price { get; }
        decimal Amount { get; }
    }
}
