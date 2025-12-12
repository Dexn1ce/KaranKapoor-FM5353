using System;


using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("asian_option")]
public class Asian_option : OptionBase
{

	[Key]	
	[Column("id")]
	public int id {get; set;}

    	public int? ratecurveid {get; set;}

	[ForeignKey("ratecurveid")]	
	public rateCurve? ratecurve {get; set;} // Navigation property


	public int underlyingid {get; set;}
	[JsonIgnore]
	[ForeignKey("underlyingid")]
	public underlying? Underlying {get; set;} //navigation property

	public Asian_option() : base() {}


    


	public Asian_option( double spotPrice, double vol,double costofcarry, double intrate, DateTime timetoexp, string? optiontyp ,double strike) : base(spotPrice,vol,costofcarry,intrate,timetoexp,optiontyp,strike) {}
	

	public override double Payoff(double[] U)
	    {
  	      double S_avg = U.Average();
			if (otyp == "call")
			{
				return Math.Max(S_avg- K, 0);
			}
			else 
			{
				return Math.Max(K - S_avg, 0);
			}
		}
	
	public double Price()
		{
			throw new NotImplementedException(
					"No closed-form Black Scholes price for arithmetic Asian options.");
		
		
		}
 }
