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
    
public class TradeAggregation : IEnumerable{
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public long TimeTrade { get; set; }
    public decimal Buy { get; set; }
    public decimal Sell { get; set; }
    public IEnumerator GetEnumerator(){
        throw new NotImplementedException();
    }
}