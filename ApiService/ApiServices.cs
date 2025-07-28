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

//Binance+
//Okx
//https://www.okx.com/api/v5/market/ticker?instId=BTC-USDT

//Huobi
//https://api.huobi.pro/market/detail/merged?symbol=btcusdt
//https://huobiapi.github.io/docs/spot/v1/en/#get-chains-information
//https://api.huobi.pro/v2/reference/currencies?currency=ltc
//https://api.huobi.pro/v1/common/symbols



//Gopax
//BitHumb
//https://www.bithumb.com/react/
//UPBit
//https://sg.upbit.com/
