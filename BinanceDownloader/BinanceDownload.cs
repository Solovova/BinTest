using Serilog;

namespace BinanceDownloader;

public class BinanceDownload{
    async Task DownloadFileAsync(string url, string filePath){
        using var httpClient = new HttpClient();
        try{
            byte[] fileBytes = await httpClient.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(filePath, fileBytes);
            Log.Information("Файл успішно збережено: {FilePath}", filePath);
        }
        catch (Exception ex){
            Log.Error("Помилка при завантаженні файлу: {ExMessage} {Url}", ex.Message, url);
        }
    }

    public async Task Download(List<string> symbolsTop100, List<DateInfo> dates){
        foreach (var symbol in symbolsTop100){
            foreach (var dateInfo in dates){
                string url = BinanceFileNameUrl.GetUrl(dateInfo, symbol);
                string downloadPath = BinanceFileNameUrl.GetDownloadPath(dateInfo, symbol);

                Directory.CreateDirectory(Path.GetDirectoryName(downloadPath) ?? throw new InvalidOperationException());

                if (File.Exists(downloadPath)){
                    Log.Information("Файл вже скачаний {DownloadPath}", downloadPath);
                }
                else{
                    await DownloadFileAsync(url, downloadPath);
                }
            }
        }
    }
}