using System.Globalization;
using System.IO.Compression;
using CsvHelper;
using Serilog;

namespace BinanceDownloader.Download;

public class BinanceCsvReader{
    static Dictionary<long, TradeAggregationCsv> AggregateTradesPerSecond(List<BinanceTradeDataRaw> trades){
        return trades.GroupBy(t => t.TimeTrade / 1000 / 1000) // Конвертуємо мілісекунди в секунди
            .ToDictionary(
                g => g.Key,
                g => new TradeAggregationCsv{
                    Price = g.Average(t => t.Price),
                    TimeTrade = g.Min(t => t.TimeTrade),
                    Buy = g.Where(t => !t.IsBuyerMaker).Sum(t => t.Quantity),
                    Sell = g.Where(t => t.IsBuyerMaker).Sum(t => t.Quantity),
                    TradesCountBuy = g.Count(t => !t.IsBuyerMaker),
                    TradesCountSell = g.Count(t => t.IsBuyerMaker),
                });
    }

    static async Task LoadCsvFile(string filePath, string symbol){
        Log.Information("Початок завантаження файлу: {FilePath}", filePath);
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Підрахунок загальної кількості рядків
        reader.BaseStream.Position = 0;
        reader.DiscardBufferedData();

        var trades = new List<BinanceTradeDataRaw>();

        while (csv.Read()){
            var trade = new BinanceTradeDataRaw{
                Price = csv.GetField<decimal>(1),
                Quantity = csv.GetField<decimal>(2),
                TimeTrade = csv.GetField<long>(4),
                IsBuyerMaker = csv.GetField<bool>(5),
            };
            trades.Add(trade);
        }

        Log.Information("Завантаження завершено. Всього оброблено {Rows} рядків", trades.Count);

        var results = AggregateTradesPerSecond(trades);

        Log.Information("Конвертування завершено. Всього конвертовано {Rows} рядків", results.Count);
        var connectionString = BinanceContext.GetDbConnectingString();
        var binanceToPostgre = new BinanceDbAggTrade(connectionString);
        await binanceToPostgre.LoadCsvFile(symbol, results);
    }

    static string ExtractFile(string toDir, string nameOfArchive){
        string lastFullPath = String.Empty;
        try{
            // Створюємо директорію призначення, якщо її не існує
            Directory.CreateDirectory(toDir);
            using (var archive = ZipFile.OpenRead(nameOfArchive)){
                foreach (ZipArchiveEntry entry in archive.Entries){
                    // Створюємо повний шлях для файлу
                    string fullPath = Path.Combine(toDir, entry.FullName);
                    string directory = Path.GetDirectoryName(fullPath)!;

                    // Створюємо підкаталоги, якщо потрібно
                    if (!string.IsNullOrEmpty(directory)){
                        Directory.CreateDirectory(directory);
                    }

                    // Пропускаємо, якщо це директорія
                    if (string.IsNullOrEmpty(entry.Name)) continue;

                    // Розархівовуємо файл
                    entry.ExtractToFile(fullPath, true);
                    lastFullPath = fullPath;
                }
            }

            Log.Information("Архів успішно розпаковано: {ArchivePath}", nameOfArchive);
        }
        catch (Exception ex){
            Log.Error(ex, "Помилка при розархівуванні файлу: {ArchivePath}", nameOfArchive);
            //throw;
        }
        return lastFullPath;
    }

    public static async Task ExtractMany(List<string> symbolsTop100, List<DateInfo> dates){
        foreach (var symbol in symbolsTop100){
            foreach (var dateInfo in dates){
                string toDir = "D:\\Downloads\\CSV\\";
                string nameOfArchive = BinanceContext.GetDownloadPath(dateInfo, symbol);
                if (File.Exists(nameOfArchive)){
                    string nameOfFile = ExtractFile(toDir, nameOfArchive);
                    await LoadCsvFile(nameOfFile, $"{symbol}USDT");
                    File.Delete(nameOfFile);
                }
            }
        }
    }
}