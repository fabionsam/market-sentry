namespace MarketSentry.Core.Entities
{
    public class ApiConfiguration
    {
        public int Id { get; set; }
        public string ProviderName { get; set; } = string.Empty; // Ex: "Brapi", "AlphaVantage"
        public string BaseUrl { get; set; } = string.Empty;      // Ex: "https://brapi.dev"
        public string ApiToken { get; set; } = string.Empty;     // segredo
        public bool IsActive { get; set; }                       // Está em uso?
    }
}