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


var symbolsTop100 = BinanceSymbolTop.GetTop100ByVolumeList();
var dates = BinanceDate.GetDateFromRange(new DateTime(2025, 5, 1), new DateTime(2025, 5, 31));//last

var binanceDownload = new BinanceDownload();
await binanceDownload.DownloadMany(symbolsTop100, dates);

//await BinanceChecksums.VerifyAllChecksums("D:\\Downloads\\2025-05\\");

//await BinanceCsvReader.ExtractMany(symbolsTop100, dates);


//pg_dump -U postgres -Fc binance -f d:\binance_2025_06_01_2025_07_27_1s.dump
//ToDo дозавантаження по дню
//ToDo зробити автоагрегацію 1м, 5м, 15м, 1г, 4г, 1д
//ToDo малювання графігу
//ToDo навігація по графіку масштаб, стартове положення, період ,кінцеве положення, переміщення вікна період
