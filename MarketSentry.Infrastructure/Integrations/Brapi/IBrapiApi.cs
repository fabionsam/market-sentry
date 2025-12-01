using Refit;

namespace MarketSentry.Infrastructure.Integrations.Brapi
{
    public interface IBrapiApi
    {
        // A Brapi usa a rota /api/quote/{tickers}
        [Get("/api/quote/{symbol}")]
        Task<BrapiResponse> GetQuoteAsync(string symbol, [AliasAs("token")] string token);
    }
}