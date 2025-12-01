using Newtonsoft.Json;

namespace MarketSentry.Infrastructure.Integrations.Brapi
{
    public class BrapiResponse
    {
        [JsonProperty("results")]
        public List<BrapiResult> Results { get; set; }
    }

    public class BrapiResult
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("regularMarketPrice")]
        public decimal RegularMarketPrice { get; set; }
    }
}