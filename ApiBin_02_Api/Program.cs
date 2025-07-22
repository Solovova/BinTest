using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

static async Task<string> GetPrice(){
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


Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

await GetSym();
return;

static async Task GetSym(){
    string json = await GetPrice();
    Console.WriteLine(json);
    PriceInfo? info = JsonSerializer.Deserialize<PriceInfo>(json);
    Console.WriteLine($"Символ: {info!.symbol}");
    Console.WriteLine($"Ціна: {info.price}");

    decimal price = decimal.Parse(info.price ?? throw new InvalidOperationException(),
        System.Globalization.CultureInfo.InvariantCulture);
    Console.WriteLine($"Ціна: {price}");
}

public class PriceInfo{
    // ReSharper disable once InconsistentNaming
    public string? symbol{ get; set; }

    // ReSharper disable once InconsistentNaming
    public string? price{ get; set; }
}