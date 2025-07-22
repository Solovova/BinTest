namespace ApiBin_01;

using Microsoft.Extensions.Configuration;



public class ProgramApiGetSecret{
    public async Task Run(){
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

// Завантаження конфігурації з user-secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

// Отримання ключів
        string? apiKey = configuration["BinanceApiKey"];
        string? apiSecret = configuration["BinanceApiSecret"];

// Перевірка наявності ключів
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret)){
            Console.WriteLine("API ключ або секрет не знайдено в user-secrets.");
            Console.WriteLine("Будь ласка, налаштуйте їх за допомогою команд:");
            Console.WriteLine("dotnet user-secrets set \"BinanceApiKey\" \"your_api_key_here\"");
            Console.WriteLine("dotnet user-secrets set \"BinanceApiSecret\" \"your_api_secret_here\"");
            return;
        }

        Console.WriteLine("Секретні ключі успішно завантажено.");
        
        
        
// Створення екземпляра сервісу та запуск
        var binanceApi = new ProgramApiGetSecretWork(apiKey, apiSecret);
         await binanceApi.Run();

    }
}




