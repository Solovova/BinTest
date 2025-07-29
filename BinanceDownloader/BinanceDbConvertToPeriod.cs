using Npgsql;
using Serilog;

namespace BinanceDownloader;

public class BinanceDbConvertToPeriod{
    public class TableMinMax{
        public long MinDate{ get; set; }
        public long MaxDate{ get; set; }
    }

    public static void DropAllPeriodTables(){
        var symbols = BinanceSymbolTop.GetTop100ByVolumeList();
        using (var connection = new NpgsqlConnection(BinanceFileNameUrl.GetDbConnectingString())){
            connection.Open();
            foreach (var symbol in symbols){
                foreach (var period in BinanceFileNameUrl.GraphPeriodDb.Keys){
                    string tableName = BinanceFileNameUrl.GetDbTableName(symbol, period);
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

    public static TableMinMax GetTableMinMaxDates(string symbol, string period = ""){
        var tableName = BinanceFileNameUrl.GetDbTableName(symbol, period);
        using (var connection = new NpgsqlConnection(BinanceFileNameUrl.GetDbConnectingString())){
            connection.Open();

            string query = @"
            SELECT 
                MIN(trade_time) as min_date,
                MAX(trade_time) as max_date
            FROM " + tableName;

            using (var command = new NpgsqlCommand(query, connection)){
                using (var reader = command.ExecuteReader()){
                    if (reader.Read()){
                        return new TableMinMax{
                            MinDate = reader.GetInt64(0),
                            MaxDate = reader.GetInt64(1)
                        };
                    }
                }
            }
        }

        throw new Exception($"Не вдалося отримати дані з таблиці {tableName}");
    }
}