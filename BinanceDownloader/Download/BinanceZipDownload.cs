using Serilog;

namespace BinanceDownloader.Download;

public class BinanceZipDownload{
    async Task DownloadFileAsync(string url, string filePath, HttpClient httpClient){
        //using var httpClient = new HttpClient();
        try{
            byte[] fileBytes = await httpClient.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(filePath, fileBytes);
            Log.Information("Файл успішно збережено: {FilePath}", filePath);
        }
        catch (Exception ex){
            Log.Error("Помилка при завантаженні файлу: {ExMessage} {Url}", ex.Message, url);
        }
    }

    async Task Download(string url, string downloadPath, HttpClient httpClient){
        Directory.CreateDirectory(Path.GetDirectoryName(downloadPath) ?? throw new InvalidOperationException());

        if (File.Exists(downloadPath)){
            Log.Information("Файл вже скачаний {DownloadPath}", downloadPath);
        }
        else{
            await DownloadFileAsync(url, downloadPath, httpClient);
        }
    }

    public async Task DownloadMany(List<string> symbolsTop100, List<DateInfo> dates){
        var tasks = new List<Task>();
        var handler = new SocketsHttpHandler{
            MaxConnectionsPerServer = 5, // Встановлюємо максимальну кількість з'єднань
            PooledConnectionLifetime = TimeSpan.FromMinutes(2), // Опціонально: час життя з'єднання в пулі
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1) // Опціонально: час простою з'єднання
        };

        HttpClient httpClient = new HttpClient(handler);
        httpClient.Timeout = TimeSpan.FromSeconds(30000);

        foreach (var symbol in symbolsTop100){
            foreach (var dateInfo in dates){
                string url = BinanceContext.GetUrl(dateInfo, symbol);
                string downloadPath = BinanceContext.GetDownloadPath(dateInfo, symbol);

                tasks.Add(Download(url, downloadPath, httpClient));
                tasks.Add(Download($"{url}.CHECKSUM", $"{downloadPath}.CHECKSUM", httpClient));
            }
        }

        await Task.WhenAll(tasks);
    }
}