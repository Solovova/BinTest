namespace BinanceDownloader;

public class DateInfo{
    public string Day{ get; init; } = string.Empty;
    public string Month{ get; init; } = string.Empty;
    public string Year{ get; init; } = string.Empty;
}

public class BinanceDate{
    public static List<DateInfo> GetDateFromRange(DateTime startDate, DateTime endDate){
        var dates = new List<DateInfo>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1)){
            dates.Add(new DateInfo{Day = $"{date.Day:D2}", Month = $"{date.Month:D2}", Year = $"{date.Year:D4}"});
        }

        return dates;
    }
}