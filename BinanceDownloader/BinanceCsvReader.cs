using System.Globalization;
using CsvHelper;
using Serilog;

namespace BinanceDownloader;

public class BinanceCsvReader{
    
    
    public Dictionary<long, TradeAggregation> AggregateTradesPerSecond(List<BinanceTradeDataRaw> trades)
    {
        return trades.GroupBy(t => t.TimeTrade/1000/1000) // Конвертуємо мілісекунди в секунди
            .ToDictionary(
                g => g.Key,
                g => new TradeAggregation
                {
                    Price = g.Average(t => t.Price),
                    Quantity = g.Sum(t => t.Quantity),
                    TimeTrade = g.Min(t => t.TimeTrade),
                    Buy = g.Where(t => t.IsBuyerMaker).Sum(t => t.Quantity),
                    Sell = g.Where(t => !t.IsBuyerMaker).Sum(t => t.Quantity)
                });
    }



    public async Task LoadCsvFile(string filePath, string symbol){
        Log.Information("Початок завантаження файлу: {FilePath}", filePath);
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Підрахунок загальної кількості рядків
        reader.BaseStream.Position = 0;
        reader.DiscardBufferedData();

        var trades = new List<BinanceTradeDataRaw>();

        while (csv.Read()){
            var trade = new BinanceTradeDataRaw{
                Id = csv.GetField<long>(0),
                Price = csv.GetField<decimal>(1),
                Quantity = csv.GetField<decimal>(2),
                QuoteQuantity = csv.GetField<decimal>(3),
                TimeTrade = csv.GetField<long>(4),
                IsBuyerMaker = csv.GetField<bool>(5),
                IsBestMatch = csv.GetField<bool>(6)
            };
            trades.Add(trade);
        }

        Log.Information("Завантаження завершено. Всього оброблено {Rows} рядків", trades.Count);
        
        var results = AggregateTradesPerSecond(trades);
        
        Log.Information("Конвертування завершено. Всього конвертовано {Rows} рядків", results.Count);
        var connectionString = "Host=localhost;Database=binance;Username=postgres;Password=vbwqu1pa";
        var bananceCvvToPostgreeAggregate = new BananceCVVToPostgreeAggregate(connectionString);
        await bananceCvvToPostgreeAggregate.LoadCsvFile(symbol,results);
    }
}