using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketSentry.Infrastructure.Integrations.HgBrasil
{
    public class HgResponse
    {
        // AQUI ESTÁ A CORREÇÃO: Mudamos de Dictionary para JsonElement
        [JsonPropertyName("results")]
        public JsonElement Results { get; set; }

        [JsonPropertyName("valid_key")]
        public bool ValidKey { get; set; }
    }

    public class HgResult
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}