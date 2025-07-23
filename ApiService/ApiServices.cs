using System.Globalization;
using System.Text.Json.Serialization;
using Serilog;

namespace ApiService;

public class ApiServices{
    public readonly Dictionary<string,ApiService> ApiServicesCollection= new ();

    public ApiServices(){
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public void AddApiService(string name){
        if (ApiServicesCollection.ContainsKey(name)) return;
        ApiServicesCollection[name] = new ApiService(name);
    }
    
    public ApiService GetApiService(string name){
        if (ApiServicesCollection.ContainsKey(name)) return ApiServicesCollection[name];
        ApiServicesCollection[name] = new ApiService(name);
        return ApiServicesCollection[name];
    }
    

}