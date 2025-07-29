using Npgsql;

namespace BinanceDownloader;

public class AggregatedData{
    public DateTime Period{ get; set; }
    public decimal OpenPrice{ get; set; }
    public decimal ClosePrice{ get; set; }
    public decimal HighPrice{ get; set; }
    public decimal LowPrice{ get; set; }
    public decimal Volume{ get; set; }
    public int TradesCount{ get; set; }
}

public class BinanceDataAggregator{
    private readonly string _connectionString;

    public BinanceDataAggregator(string connectionString){
        _connectionString = connectionString;
    }

    //public async Task AggregateAndSaveDataAsync(string sourceTable, string targetTable,
        // BinanceDbConvertToPeriod.TimePeriod period){
        // using var connection = new NpgsqlConnection(_connectionString);
        // await connection.OpenAsync();
        //
        // // Створюємо цільову таблицю, якщо вона не існує
        // await CreateTargetTableIfNotExists(connection, targetTable);
        //
        // // Виконуємо агрегацію та зберігаємо дані
        // using var transaction = await connection.BeginTransactionAsync();
        // try{
        //     string aggregationQuery = $@"
        //         INSERT INTO {targetTable} (
        //             period_start,
        //             open_price,
        //             close_price,
        //             high_price,
        //             low_price,
        //             volume,
        //             trades_count
        //         )
        //         SELECT
        //             to_timestamp({period.StartUnixTime}) as period_start,
        //             FIRST_VALUE(price) OVER (ORDER BY unix_time) as open_price,
        //             LAST_VALUE(price) OVER (ORDER BY unix_time) as close_price,
        //             MAX(price) as high_price,
        //             MIN(price) as low_price,
        //             SUM(volume) as volume,
        //             COUNT(*) as trades_count
        //         FROM {sourceTable}
        //         WHERE unix_time BETWEEN @start_time AND @end_time
        //         GROUP BY to_timestamp({period.StartUnixTime})
        //         ON CONFLICT (period_start) DO UPDATE
        //         SET
        //             open_price = EXCLUDED.open_price,
        //             close_price = EXCLUDED.close_price,
        //             high_price = EXCLUDED.high_price,
        //             low_price = EXCLUDED.low_price,
        //             volume = EXCLUDED.volume,
        //             trades_count = EXCLUDED.trades_count";
        //
        //     using var command = new NpgsqlCommand(aggregationQuery, connection, transaction);
        //     command.Parameters.AddWithValue("@start_time", period.StartUnixTime);
        //     command.Parameters.AddWithValue("@end_time", period.EndUnixTime);
        //
        //     await command.ExecuteNonQueryAsync();
        //     await transaction.CommitAsync();
        // }
        // catch (Exception){
        //     await transaction.RollbackAsync();
        //     throw;
        // }
    }

    // private async Task CreateTargetTableIfNotExists(NpgsqlConnection connection, string targetTable){
    //     string createTableQuery = $@"
    //         CREATE TABLE IF NOT EXISTS {targetTable} (
    //             period_start TIMESTAMP WITH TIME ZONE PRIMARY KEY,
    //             open_price DECIMAL(18,8) NOT NULL,
    //             close_price DECIMAL(18,8) NOT NULL,
    //             high_price DECIMAL(18,8) NOT NULL,
    //             low_price DECIMAL(18,8) NOT NULL,
    //             volume DECIMAL(18,8) NOT NULL,
    //             trades_count INTEGER NOT NULL,
    //             created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
    //         )";
    //
    //     using var command = new NpgsqlCommand(createTableQuery, connection);
    //     await command.ExecuteNonQueryAsync();
    // }
//}

// // Клас для обробки всього діапазону даних
// public class DataProcessor
// {
//     private readonly DataAggregator _aggregator;
//     
//     public DataProcessor(string connectionString)
//     {
//         _aggregator = new DataAggregator(connectionString);
//     }
//
//     public async Task ProcessDataRangeAsync(string sourceTable, string targetTable, long startUnixTime, long endUnixTime)
//     {
//         // Розбиваємо весь діапазон на періоди по добі
//         var periods = TimeRangeSplitter.SplitByDays(startUnixTime, endUnixTime);
//         
//         foreach (var period in periods)
//         {
//             Console.WriteLine($"Обробка періоду: {period.StartDate:yyyy-MM-dd HH:mm:ss} - {period.EndDate:yyyy-MM-dd HH:mm:ss}");
//             
//             try
//             {
//                 await _aggregator.AggregateAndSaveDataAsync(sourceTable, targetTable, period);
//                 Console.WriteLine($"Період успішно оброблено");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Помилка обробки періоду: {ex.Message}");
//                 // Тут можна додати логіку для повторної спроби або логування помилок
//             }
//         }
//     }
// }