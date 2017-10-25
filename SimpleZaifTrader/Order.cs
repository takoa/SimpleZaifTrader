namespace SimpleZaifTrader
{
    public class Order
    {
        public long ID { get; set; }
        public string Action { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        //public decimal Limit { get; set; }
        public string Comment { get; set; }
    }
}