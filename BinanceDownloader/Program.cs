using BinanceDownloader;
using Serilog;
using Serilog.Events;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.File("logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        encoding: System.Text.Encoding.UTF8
    )
    .CreateLogger();


//await binanceTop.GetTop100ByVolumeAsync();
//var symbolsTop100 = BinanceSymbolTop.GetTop100ByVolumeList();
//var symbolsTop100 = BinanceSymbolTop.GetListSymbols();

//var dates = BinanceDate.GetDateFromRange(new DateTime(2025, 7, 1), DateTime.Today.AddDays(-1));//last
//var dates = BinanceDate.GetDateFromRange(new DateTime(2025, 6, 1), new DateTime(2025, 6, 30));
//var binanceDownload = new BinanceDownload();
//await binanceDownload.DownloadMany(symbolsTop100, dates);

//Log.Information(BinanceCsvToBase.ExtractFile("BTC",new DateInfo{Day = "27", Month = "07", Year = "2025"}));
//BinanceCsvToBase.ExtractMany(symbolsTop100, dates);

//await BinanceChecksums.VerifyAllChecksums("D:\\Downloads\\2025-06\\");

//Завантаження в базу даних
//var symbolsTop100 = BinanceSymbolTop.GetListSymbols();
//var dates = BinanceDate.GetDateFromRange(new DateTime(2025, 6, 25), new DateTime(2025, 6, 25));
var symbolsTop100 = BinanceSymbolTop.GetTop100ByVolumeList();
var dates = BinanceDate.GetDateFromRange(new DateTime(2025, 7, 1), DateTime.Today.AddDays(-1));//last
await BinanceCsvToBase.ExtractMany(symbolsTop100, dates);


// Console.WriteLine( DateTimeOffset.FromUnixTimeMilliseconds(
//     1750809602419863/1000).DateTime);
//     //253402300799999