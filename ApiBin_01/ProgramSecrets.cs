using Microsoft.Extensions.Configuration;

// dotnet user-secrets init
//dotnet user-secrets set "MySecretApiKey" "my-value"

namespace ApiBin_01;

internal class ProgramSecrets{
    public void Run(){
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<ProgramSecrets>();
        var configuration = builder.Build();

        string? myApiKey = configuration["MySecretApiKey"];
        Console.WriteLine($"MySecretApiKey: {myApiKey}");
    }
}