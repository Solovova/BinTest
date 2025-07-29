using BinanceDownloader;
using BinanceDownloader.Download;
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
//var dates = BinanceContext.GetDateFromRange(new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));//last
//7 28 x x
//6 30 x x
//5 31 x x
//4 30 x
//3 31 x
//2 28 x
//1 31 x
//var binanceDownload = new BinanceZipDownload();
//await binanceDownload.DownloadMany(symbolsTop100, dates);

//await BinanceCsvChecksums.VerifyAllChecksums("D:\\Downloads\\2025-03\\");

//await BinanceCsvReader.ExtractMany(symbolsTop100, dates); 


//await BinanceDbConvertToPeriod.ConvertAll(symbolsTop100);

//pg_dump -U postgres -Fc binance -f d:\binance_2025_06_01_2025_07_27_1s.dump

//ToDo малювання графігу
//ToDo навігація по графіку масштаб, стартове положення, період ,кінцеве положення, переміщення вікна період

// BinanceDbConvertToPeriod.TimePeriod timePeriod = BinanceDbConvertToPeriod.GetTableMinMaxDates("BTCUSDT");
// var periods = BinanceDbConvertToPeriod.GetTimePeriods(timePeriod, "1d");
// Console.WriteLine(periods.Count); //28 30 31 = 89
