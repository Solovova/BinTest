using Serilog;

namespace BinanceDownloader;

using System.IO.Compression;

public class BinanceCsvToBase{
    public static string ExtractFile(string symbol, DateInfo dateInfo){
        string toDir = "D:\\Downloads\\CSV\\";
        string nameOfArchive = BinanceFileNameUrl.GetDownloadPath(dateInfo, symbol);
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

    public static void ExtractMany(List<string> symbolsTop100, List<DateInfo> dates){
        foreach (var symbol in symbolsTop100){
            foreach (var dateInfo in dates){
                ExtractFile(symbol, dateInfo);
            }
        }
    }
}