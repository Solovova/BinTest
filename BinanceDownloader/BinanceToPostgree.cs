using Npgsql;

namespace BinanceDownloader;

public class BinanceToPostgree{
    private readonly string _connectionString;

    public BinanceToPostgree(string connectionString){
        _connectionString = connectionString;
    }

    public async Task CreateTableIfNotExists(string symbol){
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        string tableName = BinanceFileNameUrl.GetDbTableName(symbol);
        string sql = @$"
            CREATE TABLE IF NOT EXISTS {tableName} (
                trade_time BIGINT NOT NULL PRIMARY KEY,
                price DECIMAL(20, 8) NOT NULL,
                buy DECIMAL(20, 8) NOT NULL,
                sell DECIMAL(20, 8) NOT NULL,
                trades_count_buy INT NOT NULL,
                trades_count_sell INT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS idx_{tableName}_trade_time 
            ON {tableName}(trade_time);
";
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task LoadCsvFile(string symbol, Dictionary<long, TradeAggregationCsv> tradesAggregation){
        await CreateTableIfNotExists(symbol);
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        string tableName = BinanceFileNameUrl.GetDbTableName(symbol);

        // Підготовка для масового вставлення
        using var writer = connection.BeginBinaryImport(
            $"COPY {tableName} (trade_time, price, buy, sell, trades_count_buy, trades_count_sell) " +
            "FROM STDIN (FORMAT BINARY)");

        foreach (var trade in tradesAggregation){
            writer.StartRow();
            writer.Write(trade.Value.TimeTrade);
            writer.Write(trade.Value.Price);
            writer.Write(trade.Value.Buy);
            writer.Write(trade.Value.Sell);
            writer.Write(trade.Value.TradesCountBuy);
            writer.Write(trade.Value.TradesCountSell);
        }
        await writer.CompleteAsync();
    }
}