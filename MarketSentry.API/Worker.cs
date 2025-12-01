using MarketSentry.Core.Entities;
using MarketSentry.Core.Interfaces;
using MarketSentry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace MarketSentry.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStockService _stockService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        private string _emailTemplate = string.Empty;

        // Injetando ServiceProvider porque o DbContext é "Scoped" e o Worker é "Singleton".
        // Escopo manual para pegar o banco.
        public Worker(
            ILogger<Worker> logger,
            IServiceProvider serviceProvider,
            IStockService stockService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _stockService = stockService;
            _emailService = emailService;
            _configuration = configuration;
        }

        // Executado uma vez quando o serviço inicia
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // Carrega o template HTML do disco
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "AlertTemplate.html");
            if (File.Exists(templatePath))
            {
                _emailTemplate = await File.ReadAllTextAsync(templatePath, Encoding.UTF8, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Template de e-mail não encontrado em: {path}", templatePath);
            }

            await base.StartAsync(cancellationToken);
        }

        // Loop Principal
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Iniciando ciclo de monitoramento: {time}", DateTimeOffset.Now);

                try
                {
                    // Escopo para usar o Banco de Dados
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        // 1. Busca configurações ativas no SQLite
                        var activeConfigs = await dbContext.StockConfigs
                            .Include(c => c.Api)
                            .Where(c => c.IsActive)
                            .ToListAsync(stoppingToken);

                        if (!activeConfigs.Any())
                        {
                            _logger.LogInformation("Nenhuma configuração ativa encontrada.");
                        }

                        foreach (var config in activeConfigs)
                        {
                            if (config.Api == null)
                            {
                                _logger.LogWarning("O ativo {symbol} não tem API configurada.", config.Symbol);
                                continue;
                            }

                            // 2. Busca cotação na API
                            var currentPrice = await _stockService.GetPriceAsync(config.Symbol, config.Api);

                            if (currentPrice.HasValue)
                            {
                                _logger.LogInformation("[{symbol}] Preço Atual: {price}", config.Symbol, currentPrice);

                                // 3. Salva histórico no banco
                                var history = new StockPriceHistory
                                {
                                    Symbol = config.Symbol,
                                    Price = currentPrice.Value,
                                    Timestamp = DateTime.Now
                                };
                                dbContext.StockPriceHistories.Add(history);

                                // 4. Verifica Regras e Dispara E-mail
                                await CheckRulesAndAlertAsync(config, currentPrice.Value);
                            }
                            else
                            {
                                _logger.LogWarning("Não foi possível obter cotação para {symbol}", config.Symbol);
                            }
                        }

                        // Salva o histórico de preços no banco
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro crítico no ciclo de monitoramento.");
                }

                // 5. Aguarda antes do próximo ciclo (Ex: 1 minuto)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckRulesAndAlertAsync(StockConfig config, decimal currentPrice)
        {
            string action = null;

            // Lógica de Monitoramento
            if (currentPrice <= config.PriceBuy)
            {
                action = "COMPRA (Preço abaixo do alvo)";
            }
            else if (currentPrice >= config.PriceSell)
            {
                action = "VENDA (Preço acima do alvo)";
            }

            // Se houve gatilho, prepara e envia o e-mail
            if (action != null)
            {
                _logger.LogInformation("Gatilho disparado para {symbol}: {action}", config.Symbol, action);

                if (!string.IsNullOrEmpty(config.EmailNotification))
                {
                    string body;

                    if (string.IsNullOrWhiteSpace(_emailTemplate))
                    {
                        // Se o template falhou, usa um texto simples de emergência
                        body = $@"
                            <h1>Alerta de Mercado (Modo Texto)</h1>
                            <p>O ativo <strong>{config.Symbol}</strong> disparou um alerta de <strong>{action}</strong>.</p>
                            <p>Preço Atual: {currentPrice:C2}</p>
                            <br>
                            <small>Aviso: O arquivo de template HTML não foi encontrado.</small>
                        ";

                        _logger.LogWarning("Usando layout de e-mail de emergência (Template vazio).");
                    }
                    else
                    {
                        // Usa o template bonitão
                        body = _emailTemplate
                            .Replace("{{SYMBOL}}", config.Symbol)
                            .Replace("{{ACTION}}", action)
                            .Replace("{{CURRENT_PRICE}}", currentPrice.ToString("C2", new CultureInfo("pt-BR")))
                            .Replace("{{DATE}}", DateTime.Now.ToString("g"))
                            .Replace("{{CLASS_COLOR}}", currentPrice <= config.PriceBuy ? "alert-box-green" : "alert-box-red");
                    }

                    // Monta config de SMTP 
                    var smtpConfig = new SmtpConfiguration
                    {
                        Host = "smtp.ethereal.email",
                        Port = 587,
                        UserName = "missouri.cassin58@ethereal.email",
                        Password = "AcjaRKFe3xxnhArkjp",
                        SenderEmail = "no-reply@marketsentry.com",
                        SenderName = "Market Sentry Bot"
                    };

                    await _emailService.SendEmailAsync(
                        config.EmailNotification,
                        $"Alerta de Mercado: {config.Symbol}",
                        body,
                        smtpConfig);

                    _logger.LogInformation("E-mail enviado para {email}", config.EmailNotification);
                }
            }
        }
    }
}