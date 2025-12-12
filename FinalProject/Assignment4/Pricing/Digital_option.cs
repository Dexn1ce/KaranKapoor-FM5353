using System;

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
[Table("digital_option")]
public class Digital_option : OptionBase
{
	[Key]
	[Column("id")]
	public int id {get; set;}

	[Column("payout_amount")]
	public double payout_amount{get; set;}


    	public int? ratecurveid {get; set;}

	[ForeignKey("ratecurveid")]	
	public rateCurve? Ratecurve {get; set;} // Navigation property


	public int underlyingid {get; set;}
	[JsonIgnore]
	[ForeignKey("underlyingid")]
	public underlying? underlying {get; set;} //navigation property

	public Digital_option() : base() {}

    	public Digital_option( double spotPrice, double vol,double costofcarry, double intrate, DateTime timetoexp, string? optiontyp,double strike,double pout=1.0) : base(spotPrice,vol,costofcarry,intrate,timetoexp,optiontyp,strike)
	{
		payout_amount = pout;
	}





    // ---------------------------------------------------------
    // Payoff
    // ---------------------------------------------------------
	public override double Payoff(double[] U)
	{
		double ST = U[^1];
		if (otyp == "call")
		    return (ST > K) ? payout_amount : 0.0;
		else
		    return (ST < K) ? payout_amount : 0.0;
	}

    // ---------------------------------------------------------
    // Analytical Black-Scholes price for Digital Option
    // ---------------------------------------------------------
	public double Price()
	 {
		 double tmp = sig * Math.Sqrt(T);
		 double d1 = (Math.Log(S / K) + (b + 0.5 * sig * sig) * T) / tmp;
		 double d2 = d1 - tmp;
		 
		 if (otyp == "call")
			 return payout_amount*Math.Exp(-r * T) * SpecialFunctions.N(d2);
		 else
			 return payout_amount*Math.Exp(-r * T) * SpecialFunctions.N(-d2);
	 }
/*

    public override double Delta()
    {

        double tmp = sig * Math.Sqrt(T);
        double d1 = (Math.Log(S / K) + (b + (sig * sig) * 0.5) * T) / tmp;
	double d2 = d1 - tmp;
        if (otyp == "put")
        {
           
            return -payout_amount*Math.Exp((b - r- sig*sig) * T) * (SpecialFunctions.n(d2))/ (S* sig * Math.Sqrt(T));
        }
        else
        {
            return payout_amount*Math.Exp((b - r- sig*sig) * T) * (SpecialFunctions.n(d2))/ (S* sig * Math.Sqrt(T));

        }
    }
        
    
    public override  double Gamma()
    {
        double tmp = sig*Math.Sqrt(T);

        double d1 = (Math.Log(S/K)+ (b + (sig*sig)*0.5)*T)/tmp;
	double d2 = d1 - tmp;
	double delta = Delta();

        return delta* (-SpecialFunctions.n(d2)*d2)/(S*S*sig*Math.Sqrt(T));
    }

    public override double Vega()
    {
        double tmp = sig*Math.Sqrt(T);

        double d1 = (Math.Log(S/K) + (b + (sig*sig)*0.5)*T) / tmp;
	double d2 = d1 - tmp;
        
        return payout_amount*Math.Exp(-r *T)*SpecialFunctions.n(d2)*(-d2/sig);
    }

    public override double Theta()
    {

        
            double tmp = sig*Math.Sqrt(T);

            double d1 = (Math.Log(S/K) + (b + (sig*sig)*0.5) *T)/tmp;
            double d2 = d1 - tmp;

            double t1 = -payout_amount* Math.Exp(-r*T)*SpecialFunctions.n(d2)*((b- r -sig*sig) / (sig*Math.Sqrt(T)));
            double t2 = -r * Price();

	    return t1 + t2;


}


    public virtual double Rho()
    {

        double tmp = sig*Math.Sqrt(T);

        double d1 = (Math.Log(S/K) + (b + (sig*sig)*0.5)*T)/tmp;
        double d2 = d1 - tmp;

	return -T*Price() + payout_amount*Math.Exp(-r *T)*SpecialFunctions.n(d2)*(Math.Sqrt(T)/sig);
    }



*/

}
