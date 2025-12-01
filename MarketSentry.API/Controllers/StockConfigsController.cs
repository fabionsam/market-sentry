using MarketSentry.API.DTOs;
using MarketSentry.Core.Entities;
using MarketSentry.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketSentry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockConfigsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockConfigsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StockConfigs
        [HttpGet]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDashboardDto>>> GetConfigs()
        {
            var configs = await _context.StockConfigs.Include(s => s.Api).ToListAsync();
            var dashboardData = new List<StockDashboardDto>();

            foreach (var config in configs)
            {
                // Busca o último preço salvo pelo WORKER no banco
                var lastHistory = await _context.StockPriceHistories
                    .Where(h => h.Symbol == config.Symbol)
                    .OrderByDescending(h => h.Timestamp)
                    .FirstOrDefaultAsync();

                dashboardData.Add(new StockDashboardDto
                {
                    Id = config.Id,
                    Symbol = config.Symbol,
                    PriceBuy = config.PriceBuy,
                    PriceSell = config.PriceSell,
                    ProviderName = config.Api?.ProviderName ?? "Padrão",
                    CurrentPrice = lastHistory?.Price,      
                    LastUpdate = lastHistory?.Timestamp,
                    EmailNotification = config.EmailNotification
                });
            }
            return dashboardData;
        }

        // POST: api/StockConfigs
        [HttpPost]
        public async Task<ActionResult<StockConfig>> PostConfig(StockConfig config)
        {
            // Lógica simples de Create/Update
            if (config.Id == 0)
            {
                _context.StockConfigs.Add(config);
            }
            else
            {
                _context.Entry(config).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Erro ao salvar. Verifique se o Símbolo já existe.");
            }

            return CreatedAtAction(nameof(GetConfigs), new { id = config.Id }, config);
        }

        // DELETE: api/StockConfigs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfig(int id)
        {
            var config = await _context.StockConfigs.FindAsync(id);
            if (config == null) return NotFound();

            _context.StockConfigs.Remove(config);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/StockConfigs/history/PETR4
        [HttpGet("history/{symbol}")]
        public async Task<ActionResult<IEnumerable<StockPriceHistory>>> GetHistory(string symbol)
        {
            // Pega apenas as últimas 24h
            var cutOff = DateTime.Now.AddHours(-24);
            return await _context.StockPriceHistories
                .Where(h => h.Symbol == symbol && h.Timestamp >= cutOff)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }
    }
}