using Refit;

namespace MarketSentry.Infrastructure.Integrations.HgBrasil
{
    public interface IHgBrasilApi
    {
        // Documentação HG: https://api.hgbrasil.com/finance/stock_price?key=KEY&symbol=PETR4
        [Get("/finance/stock_price")]
        Task<HgResponse> GetQuoteAsync([AliasAs("symbol")] string symbol, [AliasAs("key")] string key);
    }
}