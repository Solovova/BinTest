using System.Collections;

namespace BinanceDownloader;

public class BinanceTradeDataRaw{
    public long Id{ get; set; }
    public decimal Price{ get; set; }
    public decimal Quantity{ get; set; }
    public decimal QuoteQuantity{ get; set; }
    public long TimeTrade{ get; set; }
    public bool IsBuyerMaker{ get; set; }
    public bool IsBestMatch{ get; set; }
}
    
public class TradeAggregationCsv : IEnumerable{
    public decimal Price { get; set; }
    public long TimeTrade { get; set; }
    public decimal Buy { get; set; }
    public decimal Sell { get; set; }
    public int TradesCountBuy { get; set; }
    public int TradesCountSell { get; set; }
    
    public IEnumerator GetEnumerator(){
        throw new NotImplementedException();
    }
}

public class TradeAggregationKline{
    public long TimeTrade{ get; set; }
    public decimal OpenPrice{ get; set; }
    public decimal ClosePrice{ get; set; }
    public decimal HighPrice{ get; set; }
    public decimal LowPrice{ get; set; }
    public decimal Buy{ get; set; }
    public decimal Sell{ get; set; }
    public int TradesCountBuy { get; set; }
    public int TradesCountSell { get; set; }
}

public class DateInfo{
    public string Day{ get; init; } = string.Empty;
    public string Month{ get; init; } = string.Empty;
    public string Year{ get; init; } = string.Empty;
}

