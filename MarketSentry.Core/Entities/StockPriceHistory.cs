using System;
using System.Collections.Generic;
using System.Text;

namespace MarketSentry.Core.Entities
{
    public class StockPriceHistory
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
