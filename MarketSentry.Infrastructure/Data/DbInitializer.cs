using MarketSentry.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketSentry.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Garante que o banco existe
            context.Database.EnsureCreated();

            // 1. Upsert das APIs
            var brapiApi = UpsertApi(context, new ApiConfiguration
            {
                ProviderName = "Brapi",
                BaseUrl = "https://brapi.dev",
                ApiToken = "bT4f8q3g18EQqhAjS8JGF2",
                IsActive = true
            });

            var hgApi = UpsertApi(context, new ApiConfiguration
            {
                ProviderName = "HgBrasil",
                BaseUrl = "https://api.hgbrasil.com",
                ApiToken = "e76d3322",
                IsActive = true
            });

            // Salva as APIs para garantir que temos os IDs
            context.SaveChanges();

            //// 2. Upsert das Ações Iniciais
            //UpsertStock(context, new StockConfig
            //{
            //    Symbol = "PETR4",
            //    PriceBuy = 30.00m,
            //    PriceSell = 45.00m,
            //    EmailNotification = "fabio.ctmattos@gmail.com",
            //    IsActive = true,
            //    ApiId = brapiApi.Id
            //});

            //UpsertStock(context, new StockConfig
            //{
            //    Symbol = "VALE3",
            //    PriceBuy = 55.00m,
            //    PriceSell = 85.00m,
            //    EmailNotification = "fabio.ctmattos@gmail.com",
            //    IsActive = true,
            //    ApiId = brapiApi.Id
            //});

            //UpsertStock(context, new StockConfig
            //{
            //    Symbol = "USD", // Dólar costuma funcionar na HG Free
            //    PriceBuy = 4.80m,
            //    PriceSell = 5.20m,
            //    EmailNotification = "fabio.ctmattos@gmail.com",
            //    IsActive = true,
            //    ApiId = hgApi.Id
            //});

            //// Salva as Ações
            //context.SaveChanges();
        }

        // --- MÉTODOS AUXILIARES ---
        private static ApiConfiguration UpsertApi(AppDbContext context, ApiConfiguration seedApi)
        {
            var existingApi = context.ApiConfigurations
                .FirstOrDefault(a => a.ProviderName == seedApi.ProviderName);

            if (existingApi == null)
            {
                // Adiciona e retorna o objeto que acabamos de criar
                var entry = context.ApiConfigurations.Add(seedApi);
                return entry.Entity;
            }
            else
            {
                // Atualiza e retorna o objeto existente
                existingApi.BaseUrl = seedApi.BaseUrl;
                existingApi.ApiToken = seedApi.ApiToken;
                existingApi.IsActive = seedApi.IsActive;
                return existingApi;
            }
        }

        private static void UpsertStock(AppDbContext context, StockConfig seedStock)
        {
            // Busca pela chave única (Symbol)
            var existingStock = context.StockConfigs
                .FirstOrDefault(s => s.Symbol == seedStock.Symbol);

            if (existingStock == null)
            {
                context.StockConfigs.Add(seedStock);
            }
            else
            {
                existingStock.ApiId = seedStock.ApiId; 
                existingStock.PriceBuy = seedStock.PriceBuy;
                existingStock.PriceSell = seedStock.PriceSell;
                existingStock.EmailNotification = seedStock.EmailNotification;
            }
        }
    }
}