using MarketSentry.Core.Entities;
using MarketSentry.Core.Interfaces;

namespace MarketSentry.Infrastructure.Services
{
    public class SmartStockService : IStockService
    {
        private readonly BrapiStockService _brapiService;
        private readonly HgBrasilStockService _hgService;

        // Injetamos os dois serviços concretos
        public SmartStockService(BrapiStockService brapiService, HgBrasilStockService hgService)
        {
            _brapiService = brapiService;
            _hgService = hgService;
        }

        public async Task<decimal?> GetPriceAsync(string symbol, ApiConfiguration apiConfig)
        {
            if (apiConfig == null) return null;

            // Decisão baseada no NOME do provedor salvo no banco
            switch (apiConfig.ProviderName.ToLower())
            {
                case "brapi":
                    return await _brapiService.GetPriceAsync(symbol, apiConfig.ApiToken, apiConfig.BaseUrl);

                case "hgbrasil":
                    return await _hgService.GetPriceAsync(symbol, apiConfig.ApiToken, apiConfig.BaseUrl);

                default:
                    Console.WriteLine($"API desconhecida: {apiConfig.ProviderName}");
                    return null;
            }
        }
    }
}