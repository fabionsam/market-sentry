using MarketSentry.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketSentry.Infrastructure.Data
{
    // Herda de DbContext, que é a base do Entity Framework
    public class AppDbContext : DbContext
    {
        // Construtor que permite passar configurações (como a string de conexão) vindo lá do Worker
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Estas props são as tabelas do banco
        public DbSet<StockConfig> StockConfigs { get; set; }
        public DbSet<StockPriceHistory> StockPriceHistories { get; set; }
        public DbSet<ApiConfiguration> ApiConfigurations { get; set; }
        public DbSet<SmtpConfiguration> SmtpConfigurations { get; set; }

        // Detalhes da tabela
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuração da Tabela StockConfig ---
            modelBuilder.Entity<StockConfig>(entity =>
            {
                // Torna a propriedade Symbol obrigatória
                entity.Property(e => e.Symbol).IsRequired();

                // CRIANDO A RESTRIÇÃO UNIQUE
                entity.HasIndex(e => e.Symbol).IsUnique();
            });

            // --- Configuração da Tabela ApiConfiguration ---
            modelBuilder.Entity<ApiConfiguration>(entity =>
            {
                entity.Property(e => e.ProviderName).IsRequired();

                // Garante que não existam duas APIs com o mesmo nome
                entity.HasIndex(e => e.ProviderName).IsUnique();
            });
        }
    }
}
