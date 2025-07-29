using System.Collections.Concurrent;
using System.Security.Cryptography;
using Serilog;

namespace BinanceDownloader.Download;



public class BinanceCsvChecksums
{
    public static async Task VerifyAllChecksums(string rootFolder)
    {
        try
        {
            var files = Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories);
            var results = new ConcurrentDictionary<string, bool>();
            
            await Parallel.ForEachAsync(files, async (file, token) =>
            {
                try
                {
                    //string md5 = await CalculateMD5(file);
                    //string sha256 = await CalculateSHA256(file);
                    
                    var checksumFile = file + ".CHECKSUM";
                    if (File.Exists(checksumFile))
                    {
                        // var savedChecksums = await File.ReadAllLinesAsync(checksumFile);
                        // bool md5Matches = savedChecksums.Any(l => l.StartsWith("MD5:") && 
                        //     l.Split(':')[1].Trim() == md5);
                        // bool sha256Matches = savedChecksums.Any(l => l.StartsWith("SHA256:") && 
                        //     l.Split(':')[1].Trim() == sha256);
                        
                        //results[file] = md5Matches && sha256Matches;
                        
                        string expectedHash = (await File.ReadAllTextAsync(checksumFile)).Trim();
                        string actualHash = await CalculateSHA256(file);
               
                        

                        bool isMatch = expectedHash.Contains(actualHash, StringComparison.OrdinalIgnoreCase);
                        results[file] = isMatch;
                        Log.Information(
                            "File: {File}, isMatch: {MD5Match}",
                            Path.GetFileName(file),
                            isMatch ? "matches" : "does not match"
                        );
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error checking file: {File}", file);
                    results[file] = false;
                }
            });

            var successful = results.Count(r => r.Value);
            var failed = results.Count(r => !r.Value);
            
            Log.Information(
                "Verification completed. Successful: {Success}, Failed: {Failures}",
                successful,
                failed
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error scanning directory");
            throw;
        }
    }

    private static async Task<string> CalculateMD5(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await md5.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private static async Task<string> CalculateSHA256(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public static async Task CreateChecksums(string filePath)
    {
        try
        {
            var md5 = await CalculateMD5(filePath);
            var sha256 = await CalculateSHA256(filePath);

            var checksumFile = filePath + ".checksum";
            await File.WriteAllLinesAsync(checksumFile, new[]
            {
                $"MD5:{md5}",
                $"SHA256:{sha256}"
            });

            Log.Information(
                "Created checksums for file: {File}\nMD5: {MD5}\nSHA256: {SHA256}",
                Path.GetFileName(filePath),
                md5,
                sha256
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating checksums for file: {File}", filePath);
            throw;
        }
    }

    public static async Task<bool> VerifyFile(string filePath)
    {
        try
        {
            var checksumFile = filePath + ".checksum";
            if (!File.Exists(checksumFile))
            {
                Log.Warning("Checksum file not found: {File}", filePath);
                return false;
            }

            var currentMD5 = await CalculateMD5(filePath);
            var currentSHA256 = await CalculateSHA256(filePath);
            
            var savedChecksums = await File.ReadAllLinesAsync(checksumFile);
            
            bool md5Matches = savedChecksums.Any(l => l.StartsWith("MD5:") && 
                l.Split(':')[1].Trim() == currentMD5);
            bool sha256Matches = savedChecksums.Any(l => l.StartsWith("SHA256:") && 
                l.Split(':')[1].Trim() == currentSHA256);

            Log.Information(
                "Verifying file {File}:\nMD5: {MD5Match}\nSHA256: {SHA256Match}",
                Path.GetFileName(filePath),
                md5Matches ? "matches" : "does not match",
                sha256Matches ? "matches" : "does not match"
            );

            return md5Matches && sha256Matches;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error verifying file: {File}", filePath);
            return false;
        }
    }
}

//await FileChecksum.VerifyAllChecksums("path/to/root/folder");
//await FileChecksum.CreateChecksums("path/to/file");
//bool result = await FileChecksum.VerifyFile("path/to/file");
