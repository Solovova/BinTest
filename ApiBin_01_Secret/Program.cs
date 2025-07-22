using Microsoft.Extensions.Configuration;

// dotnet user-secrets init
//dotnet user-secrets set "MySecretApiKey" "my-value"

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();
var configuration = builder.Build();

string? myApiKey = configuration["MySecretApiKey"];
Console.WriteLine($"MySecretApiKey: {myApiKey}");
