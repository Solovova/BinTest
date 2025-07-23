using ApiBin_01;
using Microsoft.Extensions.DependencyInjection;

//https://www.binance.com/uk-UA/binance-api

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

 //var m1 = new ProgramSecrets();
 //m1.Run();
//
 //var m2 = new ProgramApiGet();
 //await m2.Run();
 
var m3 = new ProgramApiGetSecret();
await m3.Run();


 // 1. При паралельних запитах важливо правильно налаштувати:
 // - `MaxConnectionsPerServer`
 // - `Timeout`
 // - `DefaultRequestHeaders`

 // public ProgramApiGetSecretWork(string apiKey, string apiSecret)
 // {
 //  _apiKey = apiKey;
 //  _apiSecret = apiSecret;
 //
 //  var handler = new SocketsHttpHandler
 //  {
 //   MaxConnectionsPerServer = 20, // Встановлюємо максимальну кількість з'єднань
 //   PooledConnectionLifetime = TimeSpan.FromMinutes(2), // Опціонально: час життя з'єднання в пулі
 //   PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1) // Опціонально: час простою з'єднання
 //  };
 //
 //  _httpClient = new HttpClient(handler);
 //  _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
 //  _httpClient.Timeout = TimeSpan.FromSeconds(30);
 // }
