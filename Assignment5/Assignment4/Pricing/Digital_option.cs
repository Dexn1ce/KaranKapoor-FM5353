using System;

public class Digital_option : Option
{
	public double payout_amount;
    	public Digital_option( double spotPrice, double vol, double intrate, double costofcarry, double timetoexp, string optiontyp,double strike,double pout=1.0) : base(spotPrice,vol,intrate,costofcarry,timetoexp,optiontyp,strike)
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
	public new double Price()
	 {
		 double tmp = sig * Math.Sqrt(T);
		 double d1 = (Math.Log(S / K) + (b + 0.5 * sig * sig) * T) / tmp;
		 double d2 = d1 - tmp;
		 
		 if (otyp == "call")
			 return payout_amount*Math.Exp(-r * T) * SpecialFunctions.N(d2);
		 else
			 return payout_amount*Math.Exp(-r * T) * SpecialFunctions.N(-d2);
	 }







}
