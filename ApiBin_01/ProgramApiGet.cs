using System.Text.Json;

namespace ApiBin_01;

internal class ProgramApiGet{
    public class PriceInfo{
        // ReSharper disable once InconsistentNaming
        public string? symbol{ get; set; }

        // ReSharper disable once InconsistentNaming
        public string? price{ get; set; }
    }

    async Task<string> GetPrice(){
        using (var httpClient = new HttpClient()){
            string baseUrl = "https://api.binance.com";
            string endpoint = "/api/v3/ticker/price?symbol=BTCUSDT";
            string url = baseUrl + endpoint;

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
        Console.WriteLine($"Символ: {info!.symbol}");
        Console.WriteLine($"Ціна: {info.price}");

        decimal price = decimal.Parse(info.price ?? throw new InvalidOperationException(),
            System.Globalization.CultureInfo.InvariantCulture);
        Console.WriteLine($"Ціна: {price}");
    }

    public async Task Run(){
        await GetSym();
    }
}