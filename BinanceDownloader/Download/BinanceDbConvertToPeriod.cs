using Npgsql;
using Serilog;

namespace BinanceDownloader.Download;

public class BinanceDbConvertToPeriod{
    public class TimePeriod{
        public long TimeMin{ get; set; }
        public long TimeMax{ get; set; }
    }

    public static void DropAllPeriodTables(){
        var symbols = BinanceSymbolTop.GetTop100ByVolumeList();
        using (var connection = new NpgsqlConnection(BinanceContext.GetDbConnectingString())){
            connection.Open();
            foreach (var symbol in symbols){
                foreach (var period in BinanceContext.GraphPeriodDb.Keys){
                    string tableName = BinanceContext.GetDbTableName($"{symbol}USDT", period);
                    
                    string dropQuery = $"DROP TABLE IF EXISTS {tableName}";
                    using (var command = new NpgsqlCommand(dropQuery, connection)){
                        try{
                            command.ExecuteNonQuery();
                            Log.Information("Таблицю {TableName} успішно видалено", tableName);
                        }
                        catch (Exception ex){
                            Log.Error("Помилка при видаленні таблиці {TableName}: {ExMessage}", tableName, ex.Message);
                        }
                    }
                }
            }
        }
    }

    public static TimePeriod GetTableMinMaxDates(string symbol, string period = ""){
        var tableName = BinanceContext.GetDbTableName(symbol, period);
        using (var connection = new NpgsqlConnection(BinanceContext.GetDbConnectingString())){
            connection.Open();

            string query = @"
            SELECT 
                MIN(trade_time) as min_date,
                MAX(trade_time) as max_date
            FROM " + tableName;

            using (var command = new NpgsqlCommand(query, connection)){
                using (var reader = command.ExecuteReader()){
                    if (reader.Read()){
                        return new TimePeriod{
                            TimeMin = reader.GetInt64(0),
                            TimeMax = reader.GetInt64(1)
                        };
                    }
                }
            }
        }

        throw new Exception($"Не вдалося отримати дані з таблиці {tableName}");
    }

    public static List<TimePeriod> GetTimePeriods(TimePeriod timePeriod, string period){
        var periods = new List<TimePeriod>();
        long periodLength = BinanceContext.GraphPeriodDb[period];
        var start = timePeriod.TimeMin / 1000000 / periodLength;
        start = start * periodLength * 1000000;
        var end = timePeriod.TimeMax / 1000000 / periodLength;
        end = end * periodLength * 1000000;
        for (var i = start; i <= end; i += periodLength * 1000000){
            periods.Add(new TimePeriod{
                TimeMin = i,
                TimeMax = i + periodLength * 1000000 - 1
            });
        }

        return periods;
    }

    private static async Task AggregateAndSaveDataAsync(string sourceTable, string targetTable, TimePeriod period, long periodLength){
        await using var connection = new NpgsqlConnection(BinanceContext.GetDbConnectingString());
        await connection.OpenAsync();
        await CreateTargetTableIfNotExists(connection, targetTable);
        // Виконуємо агрегацію та зберігаємо дані
        //await using var transaction = await connection.BeginTransactionAsync();
        try{
            string aggregationQuery = $@"
            INSERT INTO {targetTable} (
                trade_time,
                open_price,
                close_price,
                high_price,
                low_price,
                buy,
                sell,
                trades_count_buy,
                trades_count_sell                                       
            )
            SELECT
                group_time as trade_time,
                (array_agg(price ORDER BY trade_time))[1] as open_price,
                (array_agg(price ORDER BY trade_time DESC))[1] as close_price,
                MAX(price) as high_price,
                MIN(price) as low_price,
                SUM(buy) as buy,
                SUM(sell) as sell,
                SUM(trades_count_buy) as trades_count_buy,
                SUM(trades_count_sell) as trades_count_sell
                FROM (
                    SELECT 
                        ((trade_time / @period_length) * @period_length) as group_time,
                        trade_time,
                        price,
                        buy,
                        sell,
                        trades_count_buy,
                        trades_count_sell
                    FROM {sourceTable}
                    WHERE trade_time >= @start_time AND trade_time <= @end_time
                ) t
                GROUP BY group_time
                ORDER BY group_time;
";

            await using var command = new NpgsqlCommand(aggregationQuery, connection);
            //await using var command = new NpgsqlCommand(aggregationQuery, connection, transaction);
            
            command.Parameters.AddWithValue("@start_time", period.TimeMin);
            command.Parameters.AddWithValue("@end_time", period.TimeMax);
            command.Parameters.AddWithValue("@period_length", periodLength*1000000);

            await command.ExecuteNonQueryAsync();
            //await transaction.CommitAsync();
        }
        catch (Exception){
            //await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task CreateTargetTableIfNotExists(NpgsqlConnection connection, string targetTable){
        
        string createTableQuery = $@"
             CREATE TABLE IF NOT EXISTS {targetTable} (
                 trade_time BIGINT NOT NULL PRIMARY KEY,
                 open_price DECIMAL(18,8) NOT NULL,
                 close_price DECIMAL(18,8) NOT NULL,
                 high_price DECIMAL(18,8) NOT NULL,
                 low_price DECIMAL(18,8) NOT NULL,
                 buy DECIMAL(24, 8) NOT NULL,
                 sell DECIMAL(24, 8) NOT NULL,
                 trades_count_buy INT NOT NULL,
                 trades_count_sell INT NOT NULL
             );
             CREATE INDEX IF NOT EXISTS idx_{targetTable}_trade_time 
               ON {targetTable}(trade_time);
";

        await using var command = new NpgsqlCommand(createTableQuery, connection);
        await command.ExecuteNonQueryAsync();
        
        

    }

    public static async Task ConvertAll(List<string> symbols){
        DropAllPeriodTables();
        foreach (var symbol in symbols){
            foreach (var period in BinanceContext.GraphPeriodDb.Keys){
                string sourceTable = BinanceContext.GetDbTableName($"{symbol}USDT");
                string targetTable = BinanceContext.GetDbTableName($"{symbol}USDT", period);
                
                TimePeriod timePeriod = GetTableMinMaxDates($"{symbol}USDT");
                List<TimePeriod> periods = GetTimePeriods(timePeriod,"1d");
                Log.Information("Symbol {Symbol}, Period {Period}, Count {Count}", symbol, period, periods.Count);
                foreach (var t in periods){
                    await AggregateAndSaveDataAsync(sourceTable, targetTable, t, BinanceContext.GraphPeriodDb[period]);
                }
            }
        }
    }
}