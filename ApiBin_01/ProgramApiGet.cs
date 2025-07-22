using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiBin_01;

internal class ProgramApiGet{
    public class PriceInfo{
        [JsonPropertyName("symbol")]
        public string? Symbol{ get; init; }

        [JsonPropertyName("price")]
        public string? Price{ get; init; }
    }

    async Task<string> GetPrice(){
        using (var httpClient = new HttpClient()){
            const string baseUrl = "https://api.binance.com";
            const string endpoint = "/api/v3/ticker/price?symbol=BTCUSDT";
            const string url = baseUrl + endpoint;

            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }

    async Task GetSym(){
        string json = await GetPrice();
        Console.WriteLine(json);
        PriceInfo? info = JsonSerializer.Deserialize<PriceInfo>(json);
        Console.WriteLine($"Символ: {info!.Symbol}");
        Console.WriteLine($"Ціна: {info.Price}");

        decimal price = decimal.Parse(info.Price ?? throw new InvalidOperationException(),
            System.Globalization.CultureInfo.InvariantCulture);
        Console.WriteLine($"Ціна: {price}");
    }

    public async Task Run(){
        await GetSym();
    }
}