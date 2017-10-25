using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleZaifTrader
{
    public class CurrencyPairSettings
    {
        public string Name { get; private set; }

        public decimal PriceUnitStep { get; private set; }

        public decimal AmountUnitStep { get; private set; }

        public int OrderBookFormat { get; private set; }

        public int LastPriceFormat { get; private set; }

        public CurrencyPairSettings(string name, decimal priceUnitStep, decimal amountUnitStep, int orderBookFormat, int lastPriceFormat)
        {
            this.Name = name;
            this.PriceUnitStep = priceUnitStep;
            this.AmountUnitStep = amountUnitStep;
            this.OrderBookFormat = orderBookFormat;
            this.LastPriceFormat = lastPriceFormat;
        }
    }
}
