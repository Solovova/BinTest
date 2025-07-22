namespace ApiBin_01.Examples;

// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
//
// var configuration = new ConfigurationBuilder()
//     .AddUserSecrets<Program>()
//     .Build();
//
// string? apiKey = configuration["BinanceApiKey"];
// string? apiSecret = configuration["BinanceApiSecret"];
//
// if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
// {
//     Console.WriteLine("API ключ або секрет не знайдено в user-secrets.");
//     return;
// }
//
// var services = new ServiceCollection();
//
// // Реєструємо HttpClient з налаштуваннями
// services.AddHttpClient<BinanceApiService>((serviceProvider, client) =>
//     {
//         client.BaseAddress = new Uri("https://api.binance.com/");
//         client.Timeout = TimeSpan.FromSeconds(30);
//     })
//     .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
//     {
//         // Додаткові налаштування handler'а, якщо потрібно
//     });
//
// // Реєструємо BinanceApiService як Transient сервіс
// services.AddTransient(sp =>
// {
//     var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
//     var client = httpClientFactory.CreateClient(nameof(BinanceApiService));
//     return new BinanceApiService(client, apiKey, apiSecret);
// });
//
// var serviceProvider = services.BuildServiceProvider();
//
// // Використання сервісу
// try
// {
//     var binanceService = serviceProvider.GetRequiredService<BinanceApiService>();
//     
//     // Отримуємо інформацію про біржу
//     var exchangeInfo = await binanceService.GetExchangeInfoAsync();
//     Console.WriteLine("Exchange Info отримано успішно!");
//     
//     // Отримуємо інформацію про аккаунт
//     var accountInfo = await binanceService.GetAccountInfoAsync();
//     Console.WriteLine("Account Info отримано успішно!");
// }
// catch (HttpRequestException ex)
// {
//     Console.WriteLine($"Помилка API запиту: {ex.Message}");
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"Загальна помилка: {ex.Message}");
// }
