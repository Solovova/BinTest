namespace BinanceDownloader;

public class BinanceFileNameUrl{
    public static string GetNameOfFile(DateInfo dateInfo, string symbol){
        return $"{symbol}{"USDT"}-trades-{dateInfo.Year}-{dateInfo.Month}-{dateInfo.Day}";
    }

    public static string GetUrl(DateInfo dateInfo, string symbol){
        string stratUrl = "https://data.binance.vision/data/spot/daily/trades/";
        return $"{stratUrl}{symbol}USDT/{GetNameOfFile(dateInfo, symbol)}.zip";
    }

    public static string GetDownloadPath(DateInfo dateInfo, string symbol){
        return Path.Combine($"D:\\Downloads\\{symbol}USDT", $"{GetNameOfFile(dateInfo, symbol)}.zip");
    }
}