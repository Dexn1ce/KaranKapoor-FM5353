using System;

public class Option
{
    public double r;
    public double sig;
    public double K;
    public double T; //time till expiry
    public double b;
    public string otyp;

    public double S;

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

    public double Delta()
    {
        if (otyp == "put")
        {
            double tmp = sig * Math.Sqrt(T);
            double d1 = (Math.Log(S / K) + (b + (sig * sig) * 0.5) * T) / tmp;
            return Math.Exp((b - r) * T) * (SpecialFunctions.N(d1) - 1.0);
        }
        else
        {
            double tmp = sig * Math.Sqrt(T);
            double d1 = (Math.Log(S / K) + (b + (sig * sig) * 0.5) * T) / tmp;
            return Math.Exp((b - r) * T) * (SpecialFunctions.N(d1));

        }
    }
        
    
    public double Gamma()
    {
        double tmp = sig*Math.Sqrt(T);

        double d1 = (Math.Log(S/K)+ (b + (sig*sig)*0.5)*T)/tmp;

        return (SpecialFunctions.n(d1)*Math.Exp((b-r)*T)) / (S*tmp);
    }

    public double Vega()
    {
        double tmp = sig*Math.Sqrt(T);

        double d1 = (Math.Log(S/K) + (b + (sig*sig)*0.5)*T) / tmp;
        
        return (S*Math.Exp((b-r)*T)*SpecialFunctions.n(d1)*Math.Sqrt(T));
    }

    public virtual double Theta()
    {

        if(otyp == "put")
        {
            double tmp = sig*Math.Sqrt(T);

            double d1 = (Math.Log(S/K) + (b + (sig*sig)*0.5) *T)/tmp;
            double d2 = d1 - tmp;

            double t1 = (S* Math.Exp((b-r)*T)*SpecialFunctions.n(d1)*sig*0.5) / Math.Sqrt(T);
            double t2 = (b-r)*(S*Math.Exp((b-r)*T))*SpecialFunctions.N(-d1);
            double t3 = r * K * Math.Exp(-r * T) * SpecialFunctions.N(-d2);

            return t2 + t3 - t1;
        }
        else
        {
            double tmp = sig*Math.Sqrt(T);
            double d1 = (Math.Log(S/K) + (b + (sig*sig)*0.5) *T)/tmp;
            double d2 = d1 - tmp;

            double t1 = (S* Math.Exp((b-r)*T)*SpecialFunctions.n(d1)*sig*0.5) / Math.Sqrt(T);
            double t2 = (b-r)*(S*Math.Exp((b-r)*T))*SpecialFunctions.N(d1);
            double t3 = r * K * Math.Exp(-r * T) * SpecialFunctions.N(d2);

            return -(t1 + t2 + t3);

        }
    }

    public virtual double Rho()
    {

        double tmp = sig*Math.Sqrt(T);

        double d1 = (Math.Log(S/K) + (b + (sig*sig)*0.5)*T)/tmp;
        double d2 = d1 - tmp;
        if(otyp == "call")
        {

            if(b != 0.0)
            {
                return T*K*Math.Exp(-r*T)*SpecialFunctions.N(d2);
            }
            else
            {
                return -T*Price();
            }
        }
        else
        {
            if(b != 0.0)
            {
                return -T*K*Math.Exp(-r*T)*SpecialFunctions.N(-d2);
            }
            else
            {
                return -T*Price();
            }
        }
    }

    private double Strike()
    {

        double tmp = sig*Math.Sqrt(T);
        double d1 = (Math.Log(S/K) + (b+(sig*sig)*0.5)*T)/tmp;
        double d2 = d1 - tmp;
        if(otyp == "put")
        {

            return Math.Exp(-r*T)*SpecialFunctions.N(-d2);
        }
        else
        {


            return Math.Exp(-r*T)*SpecialFunctions.N(d2);

        }
    }

    public virtual double Payoff(double[] U)
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


    public Option()
    {
        init();
    }


    public virtual void init()
    {
        Console.Write( "Strike: ");
		K = Convert.ToDouble(Console.ReadLine());

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

		Console.Write( "1. call, 2. put: ");
		otyp = Convert.ToString(Console.ReadLine());


    }

    public Option(string optionType, string underlying)
    {
        init();
        otyp = optionType;
    }

}
