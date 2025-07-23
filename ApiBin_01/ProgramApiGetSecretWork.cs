using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;

namespace ApiBin_01;

// Моделі для десеріалізації відповіді від API
public class AssetBalance
{
    [JsonPropertyName("asset")]
    public string Asset { get; set; }

    [JsonPropertyName("free")]
    public string Free { get; set; }

    [JsonPropertyName("locked")]
    public string Locked { get; set; }
}

public class AccountInfo
{
    [JsonPropertyName("balances")]
    public List<AssetBalance> Balances { get; set; }
}

internal class ProgramApiGetSecretWork
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private const string BaseUrl = "https://api.binance.com";

    public ProgramApiGetSecretWork(string apiKey, string apiSecret)
    {
        _apiKey = apiKey;
        _apiSecret = apiSecret;
        _httpClient = new HttpClient();
        // Додаємо API ключ в заголовок для всіх запитів
        Log.Information(_apiKey);
        Log.Information(_apiSecret);
        //_httpClient.DefaultRequestHeaders.Remove("X-MBX-APIKEY");
        _httpClient.DefaultRequestHeaders.Clear();

        _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
        Console.WriteLine("Request Headers:");
        foreach (var header in _httpClient.DefaultRequestHeaders)
        {
            Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }

    }

    private string CreateSignature(string queryString)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(_apiSecret);
        byte[] queryStringBytes = Encoding.UTF8.GetBytes(queryString);
        using var hmac = new HMACSHA256(keyBytes);
        byte[] hash = hmac.ComputeHash(queryStringBytes);
        // Конвертуємо хеш в рядок шістнадцяткового формату
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
    


    
    public async Task GetAccountBalanceAsync()
    {
        Console.WriteLine("\nОтримання балансу спотового гаманця...");

        const string endpoint = "/api/v3/account";
        long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        //var recvWindow = "5000";
        //string queryString = $"timestamp={timestamp}&recvWindow={recvWindow}";
        string queryString = $"timestamp={timestamp}";
        
        // Створюємо підпис
        string signature = CreateSignature(queryString);

        string url = $"{BaseUrl}{endpoint}?{queryString}&signature={signature}";
        Log.Information(url);
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            
            string content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            //Console.WriteLine(content);
            var accountInfo = JsonSerializer.Deserialize<AccountInfo>(content);

            if (accountInfo?.Balances != null)
            {
                Log.Information("Активи з ненульовим балансом:");
                var nonZeroBalances = accountInfo.Balances
                    .Where(b => decimal.Parse(b.Free, CultureInfo.InvariantCulture) > 0 ||
                                decimal.Parse(b.Locked, CultureInfo.InvariantCulture) > 0);

                foreach (var balance in nonZeroBalances)
                {
                    Log.Information("Актив: {BalanceAsset}, Доступно: {BalanceFree}, Заблоковано: {BalanceLocked}", balance.Asset, balance.Free, balance.Locked);
                }
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Помилка запиту: {e.Message}");
            Console.WriteLine("Перевірте правильність API ключів та дозволи на них (має бути увімкнений дозвіл на читання).");
        }
    }

    public async Task GetBtcPriceAsync()
    {
        Console.WriteLine("\nОтримання ціни BTC/USDT...");
        const string endpoint = "/api/v3/ticker/price?symbol=BTCUSDT";
        string url = BaseUrl + endpoint;

        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Відповідь від API: {content}");
    }

    public async Task Run()
    {
        await GetBtcPriceAsync();
        await GetAccountBalanceAsync();
    }
}
