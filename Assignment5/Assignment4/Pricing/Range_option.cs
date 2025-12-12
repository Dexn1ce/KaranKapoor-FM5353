using System;
using System.Linq;   

public class Range_option : Option
{


    
    public Range_option( double spotPrice, double vol, double intrate, double costofcarry, double timetoexp) : base(spotPrice,vol,intrate,costofcarry,timetoexp){}

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
    public new double Price()
    {
	    throw new NotImplementedException(
		   	    "No closed-form Black Scholes price for Range options.");
    }

}
