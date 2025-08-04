namespace BinGuiDev;

public static class DevContext{
    public static readonly Dictionary<string, long> GraphPeriodDb = new Dictionary<string, long>(){
        {"1s", 1},
        {"1m", 60},
        {"5m", 300},
        {"15m", 900},
        {"1h", 3600},
        {"4h", 14400},
        {"1d", 86400}
    };
}