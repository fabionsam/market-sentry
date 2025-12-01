using Refit;
using MarketSentry.Infrastructure.Integrations.Brapi; 

namespace MarketSentry.Infrastructure.Services
{
    public class BrapiStockService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Injeta a fábrica de HTTP
        public BrapiStockService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Implementação do método GetPriceAsync
        public async Task<decimal?> GetPriceAsync(string symbol, string token, string baseUrl)
        {
            try
            {
                // 1. Cria um cliente HTTP novo
                var client = _httpClientFactory.CreateClient("DefaultClient");

                // 2. Define a URL base dinamicamente
                client.BaseAddress = new Uri(baseUrl);

                // 3. O Refit transforma esse client na Interface IBrapiApi
                var api = RestService.For<IBrapiApi>(client);

                // 4. Faz a chamada
                var response = await api.GetQuoteAsync(symbol, token);

                return response.Results?.FirstOrDefault()?.RegularMarketPrice;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar na Brapi ({baseUrl}): {ex.Message}");
                return null;
            }
        }
    }
}