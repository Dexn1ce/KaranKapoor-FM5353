using System;

public class Asian_option : Option
{
	public Asian_option() : base() {}
	

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
	
	public new double Price()
		{
			throw new NotImplementedException(
					"No closed-form Black Scholes price for arithmetic Asian options.");
		
		
		}
 }
