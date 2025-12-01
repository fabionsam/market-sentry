using MarketSentry.Core.Entities;
using MarketSentry.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketSentry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiConfigurationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiConfigurationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiConfigurations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiConfiguration>>> GetApis()
        {
            // Retorna apenas as APIs ativas para o usuário escolher
            return await _context.ApiConfigurations
                .Where(a => a.IsActive)
                .ToListAsync();
        }
    }
}