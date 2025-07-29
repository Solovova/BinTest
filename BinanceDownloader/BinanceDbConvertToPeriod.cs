using Npgsql;
using Serilog;

namespace BinanceDownloader;

public class BinanceDbConvertToPeriod{
    public void DropAllPeriodTables(){
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
}