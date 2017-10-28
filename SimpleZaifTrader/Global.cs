using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleZaifTrader
{
    public static class Global
    {
        public static readonly List<string> CurrencyPairList = new List<string>()
        {
            "BTC/JPY",
            "XEM/JPY",
            "XEM/BTC",
            "MONA/JPY",
            "MONA/BTC",
            "BCH/JPY",
            "BCH/BTC",
            "ETH/JPY",
            "ETH/BTC",
            "ZAIF/JPY",
            "ZAIF/BTC",
            "XCP/JPY",
            "XCP/BTC",
            "BCY/JPY",
            "BCY/BTC",
            "SJCX/JPY",
            "SJCX/BTC",
            "FSCC/JPY",
            "FSCC/BTC",
            "PEPECASH/JPY",
            "PEPECASH/BTC",
            "CICC/JPY",
            "CICC/BTC",
            "NCXC/JPY",
            "NCXC/BTC"
        };

        public static readonly Dictionary<string, CurrencyPairSettings> CurrencyPairDictionary
            = new Dictionary<string, CurrencyPairSettings>()
            {
                { "BTC/JPY", new CurrencyPairSettings("btc_jpy", 5m, 0.0001m, 0, 0) },
                { "XEM/JPY", new CurrencyPairSettings("xem_jpy", 0.0001m, 0.1m, 1, 0) },
                { "MONA/JPY",new CurrencyPairSettings("mona_jpy", 0.1m, 1m, 2, 0) },
                { "BCH/JPY", new CurrencyPairSettings("bch_jpy", 5m, 0.0001m, 0, 0) },
                { "ETH/JPY", new CurrencyPairSettings("eth_jpy", 5m, 0.0001m, 0, 0) },
                { "ZAIF/JPY", new CurrencyPairSettings("zaif_jpy", 0.0001m, 0.1m, 1, 0) },
                { "XCP/JPY", new CurrencyPairSettings("xcp_jpy", 0.0001m, 0.1m, 1, 0) },
                { "BCY/JPY", new CurrencyPairSettings("bitcrystals_jpy", 0.0001m, 0.1m, 1, 0) },
                { "SJCX/JPY", new CurrencyPairSettings("sjcx_jpy", 0.0001m, 0.1m, 1, 0) },
                { "FSCC/JPY", new CurrencyPairSettings("fscc_jpy", 0.0001m, 0.0001m, 1, 0) },
                { "PEPECASH/JPY", new CurrencyPairSettings("pepecash_jpy", 0.0001m, 0.0001m, 1, 0) },
                { "CICC/JPY", new CurrencyPairSettings("cicc_jpy", 0.0001m, 0.0001m, 1, 0) },
                { "NCXC/JPY", new CurrencyPairSettings("ncxc_jpy", 0.0001m, 0.0001m, 1, 0) },
                { "XEM/BTC", new CurrencyPairSettings("xem_btc", 1e-8m, 1m, 3, 1) },
                { "MONA/BTC",new CurrencyPairSettings("mona_btc", 1e-8m, 1m, 3, 1) },
                { "BCH/BTC", new CurrencyPairSettings("bch_btc", 0.0001m, 0.0001m, 3, 1) },
                { "ETH/BTC", new CurrencyPairSettings("eth_btc", 0.0001m, 0.0001m, 3, 1) },
                { "ZAIF/BTC", new CurrencyPairSettings("zaif_btc", 1e-8m, 1m, 3, 1) },
                { "XCP/BTC", new CurrencyPairSettings("xcp_btc", 1e-8m, 1m, 3, 1) },
                { "BCY/BTC", new CurrencyPairSettings("bitcrystals_btc", 1e-8m, 1m, 3, 1) },
                { "SJCX/BTC", new CurrencyPairSettings("sjcx_btc", 1e-8m, 1m, 3, 1) },
                { "FSCC/BTC", new CurrencyPairSettings("fscc_btc", 1e-8m, 1m, 3, 1) },
                { "PEPECASH/BTC", new CurrencyPairSettings("pepecash_btc", 1e-8m, 1m, 3, 1) },
                { "CICC/BTC", new CurrencyPairSettings("cicc_btc", 1e-8m, 1m, 3, 1) },
                { "NCXC/BTC", new CurrencyPairSettings("ncxc_btc", 1e-8m, 1m, 3, 1) }
            };

        public static string GetOrderBookFormat(CurrencyPairSettings settings)
        {
            switch (settings.OrderBookFormat)
            {
                case 1:
                    return "{0, 13:#######0.0000} {1, 10:####0.0000} {2, 13:#######0.0000}";
                case 2:
                    return "{0, 13:#######0.0000} {1, 10:#######0.0} {2, 13:#######0.0000}";
                case 3:
                    return "{0, 13:#######0.0000} {1:0.00000000}  {2, 13:#######0.0000}";
                default:
                    return "{0, 13:#######0.0000} {1, 10} {2, 13:#######0.0000}";
            }
        }

        public static string GetLastPriceFormat(CurrencyPairSettings settings)
        {
            switch (settings.LastPriceFormat)
            {
                case 1:
                    return "最終取引価格: {0:0.00000000}  ↕ {1:p4}";
                default:
                    return "最終取引価格: {0, 10}  ↕ {1:p4}";
            }
        }

        public static string GetTradeHistoryFormat(CurrencyPairSettings settings)
        {
            switch (settings.OrderBookFormat)
            {
                case 1:
                    return "{0} {1} {2, 10:####0.0000} {3, 13:#######0.0000}";
                case 2:
                    return "{0} {1} {2, 10:#######0.0} {3, 13:#######0.0000}";
                case 3:
                    return "{0} {1} {2:0.00000000} {3, 13:#######0.0000}";
                default:
                    return "{0} {1} {2, 10} {3, 13:#######0.0000}";
            }
        }
    }
}
