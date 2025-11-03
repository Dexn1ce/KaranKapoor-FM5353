using System;

public class Asian_option : Option
{
	int N_prices;
	public double[] S_asian;
	public Asian_option()
	{

		Console.WriteLine("Mention the the size of underlying price array: ");
		N_prices = Convert.ToInt32(Console.ReadLine());

		S_asian = new Double[N_prices];

		for (int i = 0; i < N_prices; i++)
		{
			double tmp = 0;
			tmp = Convert.ToDouble(Console.ReadLine());
			S_asian[i] = tmp;
		}
		S = S_asian.Average();


	}
	
	public override double Delta()
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
        
    
    public override double Gamma()
    {
        double tmp = sig*Math.Sqrt(T);

        double d1 = (Math.Log(S/K)+ (b + (sig*sig)*0.5)*T)/tmp;

        return (SpecialFunctions.n(d1)*Math.Exp((b-r)*T)) / (S*tmp);
    }

    public override double Vega(double U)
    {
        double tmp = sig*Math.Sqrt(T);

        double d1 = (Math.Log(S/K) + (b + (sig*sig)*0.5)*T) / tmp;
        
        return (U*Math.Exp((b-r)*T)*SpecialFunctions.n(d1)*Math.Sqrt(T));
    }

    public override double Theta(double U)
    {

        if(otyp == "put")
        {
            double tmp = sig*Math.Sqrt(T);

            double d1 = (Math.Log(U/K) + (b + (sig*sig)*0.5) *T)/tmp;
            double d2 = d1 - tmp;

            double t1 = (S* Math.Exp((b-r)*T)*SpecialFunctions.n(d1)*sig*0.5) / Math.Sqrt(T);
            double t2 = (b-r)*(U*Math.Exp((b-r)*T))*SpecialFunctions.N(-d1);
            double t3 = r * K * Math.Exp(-r * T) * SpecialFunctions.N(-d2);

            return t2 + t3 - t1;
        }
        else
        {
            double tmp = sig*Math.Sqrt(T);
            double d1 = (Math.Log(S/K) + (b + (sig*sig)*0.5) *T)/tmp;
            double d2 = d1 - tmp;

            double t1 = (S* Math.Exp((b-r)*T)*SpecialFunctions.n(d1)*sig*0.5) / Math.Sqrt(T);
            double t2 = (b-r)*(U*Math.Exp((b-r)*T))*SpecialFunctions.N(d1);
            double t3 = r * K * Math.Exp(-r * T) * SpecialFunctions.N(d2);

            return -(t1 + t2 + t3);

        }
    }

    public override double Rho(double U)
    {

        double tmp = sig*Math.Sqrt(T);

        double d1 = (Math.Log(U/K) + (b + (sig*sig)*0.5)*T)/tmp;
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

	public override double Payoff(double U)
	{
		if (otyp == "call")
		{
			return Math.Max(U- K, 0);
		}
		else
		{
			return Math.Max(K - U, 0);
		}
	}
	
	public override double Price()
	{
		double S_bar = S_asian.Average();
        if (otyp == "call")
		{
            double tmp = sig * Math.Sqrt(T);

            double d1 = (Math.Log(S_bar/ K) + (b + (sig * sig) * 0.5) * T) / tmp;
            double d2 = d1 - tmp;

            return (S_bar* Math.Exp((b - r) * T) * SpecialFunctions.N(d1) - K * Math.Exp(-r * T) * SpecialFunctions.N(d2));
        }
        else
        {
            double tmp = sig * Math.Sqrt(T);

            double d1 = (Math.Log(S_bar/ K) + (b + (sig * sig) * 0.5) * T) / tmp;
            double d2 = d1 - tmp;
            return (K * Math.Exp(-r * T) * SpecialFunctions.N(-d2) - S_bar * Math.Exp((b - r) *T) * SpecialFunctions.N(-d1));

        }
    }
	



}
