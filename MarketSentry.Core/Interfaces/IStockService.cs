using MarketSentry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketSentry.Core.Interfaces
{
    public interface IStockService
    {
        Task<decimal?> GetPriceAsync(string symbol, ApiConfiguration apiConfig);
    }
}
