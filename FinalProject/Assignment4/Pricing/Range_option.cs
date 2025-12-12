using System;
using System.Linq;   

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("range_option")]
public class Range_option : OptionBase
{

	[Key]
	[Column("id")]
	public int id {get; set;}


    	public int? ratecurveid {get; set;}

	[ForeignKey("ratecurveid")]	
	public rateCurve? Ratecurve {get; set;} // Navigation property


	public int underlyingid {get; set;}

	[JsonIgnore]
	[ForeignKey("underlyingid")]
	public underlying? underlying {get; set;} //navigation property

	public Range_option() : base() {}



	
	public Range_option( double spotPrice, double vol, double costofcarry, double intrate, DateTime timetoexp) : base(spotPrice,vol,costofcarry,intrate, timetoexp){}

    // ---------------------------------------------------------
    // Payoff = S_max - S_min
    // ---------------------------------------------------------
    public override double Payoff(double[] U)
    {
        double S_max = U.Max();
        double S_min = U.Min();

        return Math.Max(S_max - S_min, 0.0);
    }

    // ---------------------------------------------------------
    // Price = discounted expected payoff
    // ---------------------------------------------------------
    public double Price()
    {
	    throw new NotImplementedException(
		   	    "No closed-form Black Scholes price for Range options.");
    }

}
