namespace ApiBin_01.Examples;

public class BinanceApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiSecret;

    public BinanceApiService(HttpClient httpClient, string apiKey, string apiSecret)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _apiSecret = apiSecret;
        
        // Налаштування заголовків для всіх запитів
        _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
    }

    public async Task<string> GetExchangeInfoAsync()
    {
        var response = await _httpClient.GetAsync("api/v3/exchangeInfo");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetAccountInfoAsync()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var signature = CreateSignature($"timestamp={timestamp}");

        var response = await _httpClient.GetAsync($"api/v3/account?timestamp={timestamp}&signature={signature}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private string CreateSignature(string queryString)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_apiSecret));
        var signatureBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(queryString));
        return BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();
    }
}
