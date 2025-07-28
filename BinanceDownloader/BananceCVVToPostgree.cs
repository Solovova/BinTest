using System.Globalization;
using CsvHelper;
using Npgsql;
using Serilog;

namespace BinanceDownloader;



public class BinanceTradeData
{
    public long Id { get; set; }
    
    
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public decimal QuoteQuantity { get; set; }
    public DateTime Time { get; set; }
    public long TradeMils { get; set; }
    public bool IsBuyerMaker { get; set; }
    public bool IsBestMatch { get; set; }
}

public class BananceCVVToPostgree
{
    private readonly string _connectionString;
    private readonly ILogger _logger;
    
    public BananceCVVToPostgree(string connectionString, ILogger logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task CreateTableIfNotExists(string symbol)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        string tableName = GetTableName(symbol);
        string sql = @$"
            CREATE TABLE IF NOT EXISTS {tableName} (
                
                trade_id BIGINT PRIMARY KEY,
                price DECIMAL(20, 8) NOT NULL,
                quantity DECIMAL(20, 8) NOT NULL,
                quote_quantity DECIMAL(20, 8) NOT NULL,
                trade_time TIMESTAMP NOT NULL,
                trade_mils BIGINT NOT NULL,
                is_buyer_maker BOOLEAN NOT NULL,
                is_best_match BOOLEAN NOT NULL,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );
            
            
";
        
        //CREATE INDEX IF NOT EXISTS idx_{{tableName}}_trade_id 
        //ON {{tableName}}(trade_id);
        
// PRIMARY KEY
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task LoadCsvFile(string filePath, string symbol, 
        int batchSize = 10000, IProgress<double> progress = null)
    {
        _logger.Information("Початок завантаження файлу: {FilePath}", filePath);
        
        await CreateTableIfNotExists(symbol);
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        string tableName = GetTableName(symbol);
        
        // Підготовка для масового вставлення
        using var writer = connection.BeginBinaryImport(
            $"COPY {tableName} (trade_id, price, quantity, quote_quantity, trade_time, trade_mils, is_buyer_maker, is_best_match) " +
            "FROM STDIN (FORMAT BINARY)");

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        long totalRows = 0;
        long processedRows = 0;
        
        // Підрахунок загальної кількості рядків
        while (csv.Read()) totalRows++;
        reader.BaseStream.Position = 0;
        reader.DiscardBufferedData();
        
        var trades = new List<BinanceTradeData>();
        
        while (csv.Read())
        {
            //Console.WriteLine(csv.GetField<long>(4));
            List<long> ids = new List<long>();
            //Log.Information();
            
            long id = csv.GetField<long>(4)/10000000;
            
            
             if (ids.Contains(id)){
                 Log.Error($"Dublicate id: {id}");
             }
             ids.Add(id);
            
             
            var trade = new BinanceTradeData
            {
                Id = csv.GetField<long>(0),
                Price = csv.GetField<decimal>(1),
                Quantity = csv.GetField<decimal>(2),
                QuoteQuantity = csv.GetField<decimal>(3),
                Time = DateTimeOffset.FromUnixTimeMilliseconds(
                    csv.GetField<long>(4)/1000).DateTime,
                TradeMils = csv.GetField<long>(4),
                IsBuyerMaker = csv.GetField<bool>(5),
                IsBestMatch = csv.GetField<bool>(6)
            };
            
            //Console.WriteLine(trade.Id);
            
            // Записуємо дані у бінарному форматі
            writer.StartRow();
            writer.Write(trade.Id);
            writer.Write(trade.Price);
            writer.Write(trade.Quantity);
            writer.Write(trade.QuoteQuantity);
            writer.Write(trade.Time);
            writer.Write(trade.TradeMils);
            writer.Write(trade.IsBuyerMaker);
            writer.Write(trade.IsBestMatch);
            
            
            
            processedRows++;
            
            if (processedRows % 1000 == 0)
            {
                progress?.Report((double)processedRows / totalRows * 100);
                //_logger.Information("Оброблено {Rows} рядків з {Total}", 
                //    processedRows, totalRows);
            }
        }

        await writer.CompleteAsync();
        
        // Створюємо індекси після завантаження для швидкості
        await CreateIndices(connection, tableName);
        
        _logger.Information(
            "Завантаження завершено. Всього оброблено {Rows} рядків", 
            processedRows);
    }

    private async Task CreateIndices(NpgsqlConnection connection, string tableName)
    {
        // var indices = new[]
        // {
        //     $"CREATE INDEX IF NOT EXISTS idx_{tableName}_trade_time ON {tableName}(trade_time)"
        //     //$"CREATE INDEX IF NOT EXISTS idx_{tableName}_price ON {tableName}(price)",
        //     //$"CREATE INDEX IF NOT EXISTS idx_{tableName}_quantity ON {tableName}(quantity)",
        //     //$"CREATE INDEX IF NOT EXISTS idx_{tableName}_quote_quantity ON {tableName}(quote_quantity)"
        // };
        //
        // foreach (string sql in indices)
        // {
        //     await using var command = new NpgsqlCommand(sql, connection);
        //     await command.ExecuteNonQueryAsync();
        // }
    }

    private static string GetTableName(string symbol)
    {
        return $"trades_{symbol.ToLower()}";
    }

    public async Task<TradeStats> CalculateStats(string symbol, 
        DateTime? startTime = null, DateTime? endTime = null)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        string tableName = GetTableName(symbol);
        string timeFilter = "";
        
        if (startTime.HasValue || endTime.HasValue)
        {
            timeFilter = "WHERE ";
            if (startTime.HasValue)
                timeFilter += $"trade_time >= '{startTime:yyyy-MM-dd HH:mm:ss}' ";
            if (startTime.HasValue && endTime.HasValue)
                timeFilter += "AND ";
            if (endTime.HasValue)
                timeFilter += $"trade_time <= '{endTime:yyyy-MM-dd HH:mm:ss}'";
        }

        string sql = $@"
            SELECT 
                COUNT(*) as total_trades,
                MIN(price) as min_price,
                MAX(price) as max_price,
                AVG(price) as avg_price,
                SUM(quantity) as total_quantity,
                SUM(quote_quantity) as total_quote_quantity
            FROM {tableName}
            {timeFilter}";

        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new TradeStats
            {
                TotalTrades = reader.GetInt64(0),
                MinPrice = reader.GetDecimal(1),
                MaxPrice = reader.GetDecimal(2),
                AvgPrice = reader.GetDecimal(3),
                TotalQuantity = reader.GetDecimal(4),
                TotalQuoteQuantity = reader.GetDecimal(5)
            };
        }

        return null;
    }
}

public class TradeStats
{
    public long TotalTrades { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal AvgPrice { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalQuoteQuantity { get; set; }
}

// Приклад використання:
// var connectionString = "Host=localhost;Database=binance;Username=user;Password=password";
// var logger = new LoggerConfiguration()
//     .WriteTo.Console()
//     .CreateLogger();
//
// var loader = new TradeDataLoader(connectionString, logger);
//
// // Завантаження файлу з прогресом
// var progress = new Progress<double>(percent => 
//     Console.WriteLine($"Прогрес: {percent:F2}%"));
//
// await loader.LoadCsvFile(
//     "BTCUSDT-trades-2025-07-09.csv",
//     "BTCUSDT",
//     progress: progress
// );
//
// // Отримання статистики
// var stats = await loader.CalculateStats(
//     "BTCUSDT",
//     DateTime.Today.AddDays(-1),
//     DateTime.Today
// );
//
// Console.WriteLine($"""
//     Статистика торгів:
//     Всього угод: {stats.TotalTrades}
//     Мін. ціна: {stats.MinPrice}
//     Макс. ціна: {stats.MaxPrice}
//     Сер. ціна: {stats.AvgPrice}
//     Загальний об'єм: {stats.TotalQuantity}
//     Загальний об'єм в quote: {stats.TotalQuoteQuantity}
//     """);

// # Основна директорія даних
// data_directory = 'D:\PostgresData'
//
// # Додатково можна налаштувати інші директорії
// # Директорія для WAL (Write-Ahead Logging)
// wal_directory = 'D:\PostgresWAL'
//
// # Тимчасові файли
// temp_tablespaces = 'D:\PostgresTemp'

//1. - `:\Program Files\PostgreSQL\[version]\data`
//1. `postgresql.conf`
