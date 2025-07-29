using Npgsql;
using Serilog;

namespace BinanceDownloader;

public class BinanceDbConvertToPeriod{
    public class TimePeriod{
        public long TimeMin{ get; set; }
        public long TimeMax{ get; set; }
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

    public static TimePeriod GetTableMinMaxDates(string symbol, string period = ""){
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
        long periodLength = BinanceFileNameUrl.GraphPeriodDb[period];
        var start = timePeriod.TimeMin/1000000/periodLength;
        start = start * periodLength * 1000000;
        var end = timePeriod.TimeMax/1000000/periodLength;
        end = end * periodLength * 1000000;
        for (var i = start; i <= end; i += periodLength * 1000000){
            periods.Add(new TimePeriod{
                TimeMin = i,
                TimeMax = i + periodLength * 1000000 - 1
            });
        }
        return periods;
    }
}