using System;
using System.Linq;   // for Max(), Min()

public class Lookback_option : Option
{

    public Lookback_option() : base(){}


    // ---------------------------------------------------------
    // Payoff (Fixed-strike Lookback)
    // ---------------------------------------------------------
    public override double Payoff(double[] U)
    {
        double S_max = U.Max();
        double S_min = U.Min();

        if (otyp == "call")
            return Math.Max(S_max - K, 0.0);
        else
            return Math.Max(K - S_min, 0.0);
    }

    
    public new double Price()
    {
        double d1 =(Math.Log(S/K) + (r + 0.5*sig*sig)*T)/(sig*Math.Sqrt(T));
	double d2 = d1 - sig*Math.Sqrt(T);
	double d3 = d1 - (2*r*Math.Sqrt(T))/sig;

	if (otyp == "call")
	{
		return S*SpecialFunctions.N(d1) - K*Math.Exp(-r*T)*SpecialFunctions.N(d2) +( (sig*sig)/(2*r))*S*(SpecialFunctions.N(-d1) - Math.Exp(-r*T)*Math.Pow(K/S, (2*r/sig*sig))*SpecialFunctions.N(-d3));
	}
	else
	{
		return K*Math.Exp(-r*T)*SpecialFunctions.N(-d2) -S*SpecialFunctions.N(-d1) +((sig*sig)/(2*r))*S*(Math.Exp(-r*T)*Math.Pow(K/S,(2*r/sig*sig))*SpecialFunctions.N(d3) - SpecialFunctions.N(d1));
	}

    }

}
