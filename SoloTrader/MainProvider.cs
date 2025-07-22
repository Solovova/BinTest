namespace SoloTrader;

using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class MainProvider{
    public readonly BinanceApiService? BinanceService;

    public MainProvider(){
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        string? apiKey = configuration["BinanceApiKey"];
        string? apiSecret = configuration["BinanceApiSecret"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret)){
            Log.Error("API ключ або секрет не знайдено в user-secrets");
            return;
        }

        Log.Information("Секретні ключі успішно завантажено");

        var services = new ServiceCollection();

// Реєструємо HttpClient з налаштуваннями
        services.AddHttpClient<BinanceApiService>((serviceProvider, client) => {
                client.BaseAddress = new Uri("https://api.binance.com/");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler{
                // Додаткові налаштування handler'а, якщо потрібно
            });
        // Реєструємо BinanceApiService як Transient сервіс
        services.AddTransient(sp => {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient(nameof(BinanceApiService));
            return new BinanceApiService(client, apiKey, apiSecret);
        });

        var serviceProvider = services.BuildServiceProvider();

        try{
            BinanceService = serviceProvider.GetRequiredService<BinanceApiService>();
        }
        catch (HttpRequestException ex){
            Log.Error("Помилка API запиту: {ExMessage}", ex.Message);
        }
        catch (Exception ex){
            Log.Error("Загальна помилка: {ExMessage}", ex.Message);
        }
    }
}