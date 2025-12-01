using MarketSentry.Core.Entities;
using MarketSentry.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketSentry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SettingsController(AppDbContext context)
        {
            _context = context;
        }

        // --- SMTP CONFIG ---

        [HttpGet("smtp")]
        public async Task<ActionResult<SmtpConfiguration>> GetSmtp()
        {
            // Retorna a primeira configuração encontrada (assumindo que só teremos uma global)
            var config = await _context.SmtpConfigurations.FirstOrDefaultAsync();
            return config ?? new SmtpConfiguration(); // Retorna vazio se não tiver
        }

        [HttpPost("smtp")]
        public async Task<IActionResult> SaveSmtp(SmtpConfiguration config)
        {
            // Remove as antigas para garantir que só temos uma configuração ativa
            var existing = await _context.SmtpConfigurations.ToListAsync();
            _context.SmtpConfigurations.RemoveRange(existing);

            // Adiciona a nova
            _context.SmtpConfigurations.Add(config);
            await _context.SaveChangesAsync();

            return Ok(config);
        }

        // --- API CONFIG (Aproveitando o controller para gerenciar as APIs também) ---

        [HttpPost("api-provider")]
        public async Task<IActionResult> SaveApi(ApiConfiguration config)
        {
            if (config.Id == 0)
                _context.ApiConfigurations.Add(config);
            else
                _context.Entry(config).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Ok(config);
        }

        [HttpDelete("api-provider/{id}")]
        public async Task<IActionResult> DeleteApi(int id)
        {
            var api = await _context.ApiConfigurations.FindAsync(id);
            if (api == null) return NotFound();

            // Verifica se tem ações usando essa API antes de deletar (Integridade Referencial)
            var usage = await _context.StockConfigs.AnyAsync(s => s.ApiId == id);
            if (usage) return BadRequest("Não é possível deletar esta API pois existem ativos vinculados a ela.");

            _context.ApiConfigurations.Remove(api);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}