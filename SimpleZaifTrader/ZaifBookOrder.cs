using CryptoWei;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleZaifTrader
{
    public class ZaifBookOrder : IBookOrder
    {
        public decimal Price { get; private set; }
        public decimal Amount { get; private set; }

        public ZaifBookOrder()
        {
        }

        public void Update(ZaifNet.Public.Order order)
        {
            this.Price = order.Price;
            this.Amount = order.Amount;
        }

        public void Update(decimal[] order)
        {
            this.Price = order[0];
            this.Amount = order[1];
        }
    }
}
