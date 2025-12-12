

using System;


using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;



[Table("trade")]
public class Trade
{
    [Key]
    [Column("tradeid")]
    public int TradeId { get; set; }

    // What is being traded (FK)
    [Column("underlyingid")]
    public int UnderlyingId { get; set; }
    [JsonIgnore]
    public underlying? Underlying { get; set; }

    // Optional: If this is an option trade (European, Asian, Digital)
    //[Column("optionid")]
    //public int? OptionId { get; set; }
    //public Option? Option { get; set; }     // null for stock/spot trades

    // Market on which the trade is executed
    [Column("marketid")]
    public int MarketId { get; set; }
    [JsonIgnore]
    public Market? Market { get; set; }

    // Buy or Sell
    [Column("direction")]
    public string Direction { get; set; } = "BUY";   // or "SELL"

    // Number of units traded (contracts, shares)
    [Column("quantity")]
    public double Quantity { get; set; }

    // Price at which trade was executed
    [Column("tradeprice")]
    public double TradePrice { get; set; }

    // Time trade happened
    [Column("tradetime")]
    public DateTime TradeTime { get; set; }


    // Link to Trade Evaluation (PnL, Greeks at execution etc.)
    public TradeEvaluation? Evaluation { get; set; }
}

