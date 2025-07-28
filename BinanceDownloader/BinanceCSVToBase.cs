using Serilog;

namespace BinanceDownloader;

using System.IO.Compression;

public class BinanceCsvToBase{
    public static string ExtractFile(string toDir, string nameOfArchive){
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
        var progress = new Progress<double>(percent => { });
        //Console.WriteLine($"Прогрес: {percent:F2}%"));
        var connectionString = "Host=localhost;Database=binance;Username=postgres;Password=vbwqu1pa";
        //var loader = new BananceCVVToPostgree(connectionString, Log.Logger);
        var loader = new BinanceCsvReader();

        foreach (var symbol in symbolsTop100){
            foreach (var dateInfo in dates){
                string toDir = "D:\\Downloads\\CSV\\";
                string nameOfArchive = BinanceFileNameUrl.GetDownloadPath(dateInfo, symbol);
                if (File.Exists(nameOfArchive)){
                    string nameOfFile = ExtractFile(toDir, nameOfArchive);

                    await loader.LoadCsvFile(
                        nameOfFile,
                        $"{symbol}USDT"
                    );
                    
                    // await loader.LoadCsvFile(
                    //     nameOfFile,
                    //     $"{symbol}USDT",
                    //     progress: progress
                    // );
                    File.Delete(nameOfFile);
                }
            }
        }
    }
}