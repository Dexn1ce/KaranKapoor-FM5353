using System;

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[NotMapped]
public abstract class OptionBase
{
	[NotMapped]
	public double r {get; set;}	
	
	[Column("vol")]	
    	public double sig {get; set;}
	[Column("strike")]	
	public double K {get; set;}
	[Column("expirydate")]	
	public DateTime expdate {get; set;} //time till expiry
	[NotMapped]
	public double T {get; set;}

	
	public double b {get; set;}
	
	public string? otyp {get; set;}
	
	
	[NotMapped]	
	public double S {get; set;}



	internal double Timeleft()
	{
		TimeSpan diff = this.expdate - DateTime.Now;
		double totDays = diff.TotalDays;
		return totDays/ 365;
	}


	protected OptionBase() {}


    protected OptionBase( double spotPrice, double vol,double costofcarry,double intrate ,DateTime timetoexp, string? optiontyp= null,double? strike = 0)
    {
		this.K = strike ?? 0.0;
		this.S = spotPrice;

		this.sig = vol;

		this.b = costofcarry;


		expdate= timetoexp;
		T = Timeleft();

		this.otyp = optiontyp?? null;

		this.r = intrate;

    }

    public abstract double Payoff(double[] path);

}

[Table("option")]
public class Option : OptionBase
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





    public double Price()
    {
        if (otyp == "call")
        {
            double tmp = sig * Math.Sqrt(T);

            double d1 = (Math.Log(S / K) + (b + (sig * sig) * 0.5) * T) / tmp;
            double d2 = d1 - tmp;

            return (S * Math.Exp((b - r) * T) * SpecialFunctions.N(d1) - K * Math.Exp(-r * T) * SpecialFunctions.N(d2));
        }
        else
        {
            double tmp = sig * Math.Sqrt(T);

            double d1 = (Math.Log(S / K) + (b + (sig * sig) * 0.5) * T) / tmp;
            double d2 = d1 - tmp;
            return (K * Math.Exp(-r * T) * SpecialFunctions.N(-d2) - S* Math.Exp((b-r)* T) * SpecialFunctions.N(-d1));

        }
    }


    public override double Payoff(double[] U)
    {
	double ST = U[^1];
        if(otyp == "call")
        {
            return Math.Max(ST - K, 0);
        }
        else
        {
            return Math.Max(K - ST, 0);
        }
    }

    public Option(): base() {}


    public Option( double spotPrice, double vol, double costofcarry, double intrate, DateTime timetoexp, string? optiontyp= null,double? strike = 0) : base(spotPrice, vol, costofcarry, intrate, timetoexp, optiontyp, strike)
    {}



}
