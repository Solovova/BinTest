namespace BinanceDownloader;

public class BinanceFileNameUrl{
    private static string GetNameOfFile(DateInfo dateInfo, string symbol){
        return $"{symbol}{"USDT"}-trades-{dateInfo.Year}-{dateInfo.Month}-{dateInfo.Day}";
    }

    public static string GetUrl(DateInfo dateInfo, string symbol){
        string stratUrl = "https://data.binance.vision/data/spot/daily/trades/";
        return $"{stratUrl}{symbol}USDT/{GetNameOfFile(dateInfo, symbol)}.zip";
    }

    public static string GetDownloadPath(DateInfo dateInfo, string symbol){
        return Path.Combine($"D:\\Downloads\\{dateInfo.Year}-{dateInfo.Month}\\{symbol}USDT",
            $"{GetNameOfFile(dateInfo, symbol)}.zip");
    }

    public static Dictionary<string, long> GraphPeriodMain = new Dictionary<string, long>(){
        {"s", 1},
        {"m", 60},
        {"h", 3600},
        {"d", 86400}
    };
    
    public static readonly Dictionary<string, long> GraphPeriodDb = new Dictionary<string, long>(){
        {"1m", 60},
        {"5m", 300},
        {"15m", 900},
        {"1h", 3600},
        {"4h", 14400},
        {"1d", 86400}
    };
    
    public static string GetDbTableName(string symbol, string period=""){
        return string.IsNullOrEmpty(period) 
            ? $"trades_{symbol.ToLower()}" 
            : $"trades_{symbol.ToLower()}_{symbol.ToLower()}";
    }
    
    public static string GetDbConnectingString(){
        return "Host=localhost;Database=binance;Username=postgres;Password=vbwqu1pa";
    }
}