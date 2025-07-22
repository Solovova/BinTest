namespace ApiBin_01;

using Microsoft.Extensions.Configuration;
using Serilog;




public class ProgramApiGetSecret{
    public async Task Run(){
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();


// Завантаження конфігурації з user-secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

// Отримання ключів
        string? apiKey = configuration["BinanceApiKey"];
        string? apiSecret = configuration["BinanceApiSecret"];
        
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret)){
            Log.Error("API ключ або секрет не знайдено в user-secrets");
            return;
        }
        Log.Information("Секретні ключі успішно завантажено");
        
        
        
// Створення екземпляра сервісу та запуск
        var binanceApi = new ProgramApiGetSecretWork(apiKey, apiSecret);
         await binanceApi.Run();

    }
}




