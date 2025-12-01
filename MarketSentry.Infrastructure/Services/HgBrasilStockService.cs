using MarketSentry.Infrastructure.Integrations.HgBrasil;
using Refit;
using System.Text.Json;

namespace MarketSentry.Infrastructure.Services
{
    public class HgBrasilStockService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HgBrasilStockService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<decimal?> GetPriceAsync(string symbol, string token, string baseUrl)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DefaultClient");
                client.BaseAddress = new Uri(baseUrl);

                var api = RestService.For<IHgBrasilApi>(client);

                var response = await api.GetQuoteAsync(symbol, token);

                // 1. Verifica validade da chave
                if (!response.ValidKey)
                {
                    Console.WriteLine($"[HG Brasil] Erro: Chave inválida.");
                    return null;
                }

                // 2. Verificação de erro explícito no objeto 'Results'
                if (response.Results.ValueKind == JsonValueKind.Object && 
                    response.Results.TryGetProperty("error", out JsonElement errorElement) &&
                    errorElement.GetBoolean())
                {
                    string msg = "Erro desconhecido";
                    if (response.Results.TryGetProperty("message", out JsonElement msgElement))
                    {
                        msg = msgElement.GetString();
                    }

                    Console.WriteLine($"[HG Brasil] API Recusou: {msg}");
                    return null;
                }

                // 3. Tenta pegar a ação
                if (response.Results.TryGetProperty(symbol.ToUpper(), out JsonElement stockElement))
                {
                    // Como é um dicionário, precisamos buscar pela chave (Symbol)
                    var stockData = JsonSerializer.Deserialize<HgResult>(stockElement.GetRawText());
                    return stockData?.Price;
                }

                Console.WriteLine($"[HG Brasil] Ativo {symbol} não encontrado na resposta.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro HG Brasil: {ex.Message}");
                return null;
            }
        }
    }
}