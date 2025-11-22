using System;
using System.Linq;   

public class Range_option : Option
{


    public Range_option(): base(){}


    public override void init()
    {

        Console.Write("SpotPrice:");
        S = Convert.ToDouble(Console.ReadLine());

		Console.Write( "Volatility: ");
		sig = Convert.ToDouble(Console.ReadLine());

		Console.Write( "Interest rate: ");
		r = Convert.ToDouble(Console.ReadLine());

        Console.Write( "Cost of carry: ");
		b = Convert.ToDouble(Console.ReadLine());

		Console.Write( "Expiry date (in years): ");
		T = Convert.ToDouble(Console.ReadLine());
    }




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
