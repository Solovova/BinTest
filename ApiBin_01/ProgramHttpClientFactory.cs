namespace ApiBin_01;

public class ProgramHttpClientFactory{
    
    //1
// using Microsoft.Extensions.DependencyInjection;
//
// var services = new ServiceCollection();
// services.AddHttpClient(); // Додає фабрику HttpClient
// var provider = services.BuildServiceProvider();

//2
    // public class BinanceApiService
    // {
    //     private readonly HttpClient _httpClient;
    //
    //     public BinanceApiService(HttpClient httpClient)
    //     {
    //         _httpClient = httpClient;
    //     }
    
    //3
    // services.AddHttpClient<BinanceApiService>(client =>
    // {
    //     client.BaseAddress = new Uri("https://api.binance.com/");
    //     client.Timeout = TimeSpan.FromSeconds(10);
    // });


}