using MarketSentry.Core.Interfaces;
using MarketSentry.Infrastructure.Data;
using MarketSentry.Infrastructure.Services;
using MarketSentry.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Banco de Dados (Apontando para o mesmo do Worker)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configurar CORS (Permite que o Angular acesse)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200") // Porta padrão do Angular
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Configurar Injeção de Dependência para Serviços
builder.Services.AddTransient<IEmailService, MailKitEmailService>();
builder.Services.AddHttpClient();
builder.Services.AddTransient<BrapiStockService>();
builder.Services.AddTransient<HgBrasilStockService>();
builder.Services.AddTransient<IStockService, SmartStockService>();

// 4. Configurar o Worker como Hosted Service
builder.Services.AddHostedService<Worker>();


var app = builder.Build();

// Configura o Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular"); // Ativa o CORS

app.UseAuthorization();

app.MapControllers();

// --- ÁREA DE SEED (INICIALIZAÇÃO DO BANCO) 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        DbInitializer.Initialize(context);

        Console.WriteLine("Banco de dados verificado e populado com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ocorreu um erro ao popular o banco: {ex.Message}");
    }
}
// ---------------------------------------------

// 1. Permite servir arquivos estáticos (JS, CSS do Angular)
app.UseStaticFiles();

// 2. Mapeia a rota raiz para o index.html
// O 'Fallback' serve para que, se o usuário der F5 na página /settings,
// o .NET não dê erro 404, mas sim devolva o Angular para ele se virar.
app.MapFallbackToFile("index.html");

app.Run();