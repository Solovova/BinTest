using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace ApiService;

using Serilog;

public class ApiService{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    public string Name{ get; init; }

    //ToDo
    private const string BaseUrl = "https://api.binance.com";

    public ApiService(string name){
        Name = name;
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ApiServices>()
            .Build();

        // Отримання ключів
        string? apiKey = configuration[$"{name}ApiKey"];
        string? apiSecret = configuration[$"{name}ApiSecret"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret)){
            Log.Error("API ключ або секрет не знайдено в user-secrets");
            throw new InvalidOperationException("API ключ або секрет не знайдено в user-secrets");
        }

        Log.Information("Секретні ключі успішно завантажено");
        _apiKey = apiKey;
        _apiSecret = apiSecret;

        var handler = new SocketsHttpHandler{
            MaxConnectionsPerServer = 5, // Встановлюємо максимальну кількість з'єднань
            PooledConnectionLifetime = TimeSpan.FromMinutes(2), // Опціонально: час життя з'єднання в пулі
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1) // Опціонально: час простою з'єднання
        };

        _httpClient = new HttpClient(handler);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        // Налаштування заголовків для всіх запитів
        _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
    }
    
    //ToDo
    public class PriceInfo{
        [JsonPropertyName("symbol")] public string Symbol{ get; init; } = string.Empty;

        [JsonPropertyName("price")]
        public string PriceString{ get; init; } = string.Empty;
        public decimal Price => decimal.Parse(PriceString, CultureInfo.InvariantCulture);
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
    
    public async Task GetSym(){
        string json = await GetPrice();
        Console.WriteLine(json);
        PriceInfo? info = JsonSerializer.Deserialize<PriceInfo>(json);
        Console.WriteLine($"Символ: {info!.Symbol}");
        Console.WriteLine($"Ціна: {info.Price}");
    }
    
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
        Log.Information("Отримання балансу спотового гаманця...");

        const string endpoint = "/api/v3/account";
        long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        string queryString = $"timestamp={timestamp}";
        
        // Створюємо підпис
        string signature = CreateSignature(queryString);

        string url = $"{BaseUrl}{endpoint}?{queryString}&signature={signature}";

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
            Log.Error("Помилка запиту: {EMessage}", e.Message);
            Log.Error("Перевірте правильність API ключів та дозволи на них (має бути увімкнений дозвіл на читання)");
        }
    }
}