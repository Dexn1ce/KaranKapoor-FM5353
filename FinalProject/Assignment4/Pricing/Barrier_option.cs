using System;


using System.Text.Json.Serialization;
using System.Linq; // for Max/Min
using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations;
[Table("barrier_option")]
public class Barrier_option : OptionBase
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


	    // Barrier properties
	    [Column("barrierlevel")]
	    public double Barrier{get; set;}   // barrier level

	    [Column("barriertype")]
	    public string BarrierType{get; set;} =""; // "up-and-out", "down-and-out", "up-and-in", "down-and-in"

	    public Barrier_option() : base() {}

	    public Barrier_option( double spotPrice, double vol,double costofcarry, double intrate ,DateTime timetoexp, string? optiontyp,double strike,double barrierval, string barriertyp) : base(spotPrice,vol,costofcarry,intrate ,timetoexp,optiontyp,strike)
	    {
		Barrier = barrierval;

		BarrierType = barriertyp;
	    }

    // ---------------------------------------------------------
    // Check barrier condition helper
    // ---------------------------------------------------------
    private bool IsBarrierHit(double[] U)
    {
        double maxS = U.Max();
        double minS = U.Min();

        return BarrierType switch
        {
            "up-and-out"   => maxS >= Barrier,
            "down-and-out" => minS <= Barrier,
            "up-and-in"    => maxS >= Barrier,
            "down-and-in"  => minS <= Barrier,
            _              => false
        };
    }

    // ---------------------------------------------------------
    // Payoff
    // ---------------------------------------------------------
    public override double Payoff(double[] U)
    {
        bool hit = IsBarrierHit(U);
        double ST = U[^1]; // terminal price

        double vanillaPayoff = otyp == "call" ? Math.Max(ST - K, 0.0) : Math.Max(K - ST, 0.0);

        // Out options: payoff is 0 if barrier hit
        if (BarrierType.Contains("out"))
        {
            return hit ? 0.0 : vanillaPayoff;
        }
        // In options: payoff exists only if barrier hit
        else if (BarrierType.Contains("in"))
        {
            return hit ? vanillaPayoff : 0.0;
        }
        else
        {
            // Invalid type — behave as vanilla
            return vanillaPayoff;
        }
    }

    // ---------------------------------------------------------
    // Price (approximate via standard Black-Scholes on terminal price)
    // ---------------------------------------------------------
    public double Price(double[] U)
    {
        double ST = U[^1];
        double tmp = sig * Math.Sqrt(T);
        double d1 = (Math.Log(ST / K) + (b + 0.5 * sig * sig) * T) / tmp;
        double d2 = d1 - tmp;

        double vanillaPrice = otyp == "call"
            ? ST * Math.Exp((b - r) * T) * SpecialFunctions.N(d1) - K * Math.Exp(-r * T) * SpecialFunctions.N(d2)
            : K * Math.Exp(-r * T) * SpecialFunctions.N(-d2) - ST * Math.Exp((b - r) * T) * SpecialFunctions.N(-d1);

        // Apply simple barrier rule
        bool hit = IsBarrierHit(U);

        if (BarrierType.Contains("out"))
            return hit ? 0.0 : vanillaPrice;
        else if (BarrierType.Contains("in"))
            return hit ? vanillaPrice : 0.0;
        else
            return vanillaPrice;
    }

}
