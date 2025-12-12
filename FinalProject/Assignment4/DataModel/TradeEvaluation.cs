
using System;


using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


[Table("trade_evaluation")]
public class TradeEvaluation
{
    [Key]
    [Column("evaluationid")]
    public int TradeEvaluationId { get; set; }
    public int TradeId { get; set; }

    [Column("pnlatexecution")]
    public double PnlAtExecution {get; set;}

    [Column("currentpnl")]
    public double CurrentPnl {get; set;}

    public double Delta { get; set; }
    public double Gamma { get; set; }
    public double Vega { get; set; }
    public double Theta { get; set; }
    public double Rho { get; set; }

    public double ExecPrice {get; set;}

    public double CurrentPrice{get; set;}

    public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;

    public Trade? Trade { get; set; }
}





