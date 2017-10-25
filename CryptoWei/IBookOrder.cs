using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWei
{
    public interface IBookOrder
    {
        decimal Price { get; }
        decimal Amount { get; }
    }
}
