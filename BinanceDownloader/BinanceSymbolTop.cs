using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;

namespace BinanceDownloader;

public class TickerInfo{
    [JsonPropertyName("symbol")] public string Symbol{ get; set; } = string.Empty;

    [JsonPropertyName("volume")] public string Volume{ get; set; } = string.Empty;

    [JsonPropertyName("quoteVolume")] public string QuoteVolume{ get; set; } = string.Empty;

    [JsonPropertyName("lastPrice")] public string LastPrice{ get; set; } = string.Empty;

    [JsonPropertyName("priceChangePercent")]
    public string PriceChangePercent{ get; set; } = string.Empty;
}

public class BinanceSymbolTop{
    public static async Task GetTop100ByVolumeAsync(){
        try{
            const string endpoint = "/api/v3/ticker/24hr";
            string url = $"{"https://api.binance.com"}{endpoint}";

            var handler = new SocketsHttpHandler{
                MaxConnectionsPerServer = 20,
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
                EnableMultipleHttp2Connections = true
            };

            var httpClient = new HttpClient(handler);

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var allTickers = JsonSerializer.Deserialize<List<TickerInfo>>(content);

            if (allTickers == null){
                Log.Error("Не вдалося отримати дані про тікери");
                return;
            }

            var top100 = allTickers
                .Where(t => t.Symbol.EndsWith("USDT")) // Фільтруємо тільки USDT пари
                .OrderByDescending(t => decimal.Parse(t.QuoteVolume, CultureInfo.InvariantCulture))
                .Take(100);

            Log.Information("Топ 100 криптовалют за об'ємом торгів (USDT):");
            int rank = 1;
            foreach (var ticker in top100){
                var symbol = ticker.Symbol.Replace("USDT", "");
                // var volume = decimal.Parse(ticker.QuoteVolume, CultureInfo.InvariantCulture);
                // var price = decimal.Parse(ticker.LastPrice, CultureInfo.InvariantCulture);
                // var change = decimal.Parse(ticker.PriceChangePercent, CultureInfo.InvariantCulture);

                // Log.Information(
                //     "{Rank}. {Symbol,-10} Ціна: ${Price,-15:N2} Об'єм: ${Volume,-20:N2} Зміна: {Change,6:N2}%",
                //     rank++,
                //     symbol,
                //     price,
                //     volume,
                //     change
                // );
                Console.WriteLine($"\"{symbol}\",");
            }
        }
        catch (HttpRequestException e){
            Log.Error("Помилка запиту: {EMessage}", e.Message);
        }
        catch (JsonException e){
            Log.Error("Помилка парсингу JSON: {EMessage}", e.Message);
        }
    }

    public static List<string> GetTop100ByVolumeList(){
        return new List<string>{
            "ETH",
            "BTC",
            "SOL",
            "XRP",
            "BNB",
            "ERA",
            "SUI",
            "FDUSD",
            "DOGE",
            "ENA",
            "PENGU",
            "PEPE",
            "BONK",
            "OP",
            "ADA",
            "TRX",
            "CAKE",
            "HBAR",
            "BCH",
            "AVAX",
            "ASR",
            "WIF",
            "CRV",
            "UNI",
            "LINK",
            "LTC",
            "ATM",
            "ALT",
            "ARB",
            "WLD",
            "USD1",
            "SEI",
            "TRUMP",
            "AAVE",
            "XLM",
            "FLOKI",
            "MAV",
            "SPK",
            "FET",
            "NEAR",
            "KNC",
            "OMNI",
            "RESOLV",
            "TAO",
            "ONDO",
            "JUP",
            "APT",
            "ETHFI",
            "CFX",
            "PNUT",
            "NEIRO",
            "SYRUP",
            "HYPER",
            "LDO",
            "ENS",
            "DOT",
            "JUV",
            "1000CAT",
            "ORDI",
            "HAEDAL",
            "RAY",
            "CETUS",
            "C",
            "S",
            "SHIB",
            "TON",
            "SANTOS",
            "VIRTUAL",
            "TIA",
            "ACM",
            "GLM",
            "EUR",
            "PENDLE",
            "INJ",
            "ETC",
            "THE",
            "RUNE",
            "EIGEN",
            "REZ",
            "WCT",
            "ALGO",
            "ICP",
            "GALA",
            "HFT",
            "OM",
            "XUSD",
            "CITY",
            "PAXG",
            "OG",
            "PSG",
            "CVX",
            "BERA",
            "MKR",
            "INIT",
            "LISTA",
            "POL",
            "BAR",
            "RENDER"
        };
    }
}