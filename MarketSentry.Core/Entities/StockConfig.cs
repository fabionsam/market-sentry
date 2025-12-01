using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MarketSentry.Core.Entities
{
    public class StockConfig
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal PriceSell { get; set; }
        public decimal PriceBuy { get; set; }
        public string EmailNotification { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int ApiId { get; set; }

        [ForeignKey("ApiId")]
        public virtual ApiConfiguration? Api { get; set; }
    }
}
