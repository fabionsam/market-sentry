namespace MarketSentry.API.DTOs
{
    public class StockDashboardDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal PriceBuy { get; set; }
        public decimal PriceSell { get; set; }
        public decimal? CurrentPrice { get; set; }
        public DateTime? LastUpdate { get; set; } 
        public string ProviderName { get; set; } = string.Empty;
        public string EmailNotification { get; set; } = string.Empty;
    }
}