using System;
using System.Linq;   // for Max(), Min()

public class Lookback_option : Option
{
    int N_prices;
    public double[] S_path;

    public Lookback_option()
    {

        Console.WriteLine("Mention the size of underlying price path: ");
        N_prices = Convert.ToInt32(Console.ReadLine());
        S_path = new double[N_prices];

        Console.WriteLine("Enter underlying prices along the path:");
        for (int i = 0; i < N_prices; i++)
        {
            Console.Write($"Price {i + 1}: ");
            S_path[i] = Convert.ToDouble(Console.ReadLine());
        }
        S = S_path[0];
    }

    // ---------------------------------------------------------
    // Payoff (Fixed-strike Lookback)
    // ---------------------------------------------------------
    public override double Payoff(double dummy = 0.0)
    {
        double S_max = S_path.Max();
        double S_min = S_path.Min();

        if (otyp == "call")
            return Math.Max(S_max - K, 0.0);
        else
            return Math.Max(K - S_min, 0.0);
    }

    // ---------------------------------------------------------
    // Price (discounted payoff; used in MC simulation)
    // ---------------------------------------------------------
    public override double Price()
    {
        return Math.Exp(-r * T) * Payoff();
    }

    // ---------------------------------------------------------
    // Greeks (finite-difference approximations)
    // ---------------------------------------------------------
    public override double Delta()
    {
        double eps = 1e-4;
        double[] up = S_path.Select(x => x * (1 + eps)).ToArray();
        double[] down = S_path.Select(x => x * (1 - eps)).ToArray();

        double upPayoff = PayoffForPath(up);
        double downPayoff = PayoffForPath(down);

        double avgS = S_path.Average();
        return (upPayoff - downPayoff) / (2 * avgS * eps);
    }

    public override double Gamma()
    {
        double eps = 1e-4;
        double[] up = S_path.Select(x => x * (1 + eps)).ToArray();
        double[] down = S_path.Select(x => x * (1 - eps)).ToArray();

        double basePayoff = Payoff();
        double upPayoff = PayoffForPath(up);
        double downPayoff = PayoffForPath(down);

        double avgS = S_path.Average();
        return (upPayoff - 2 * basePayoff + downPayoff) / (avgS * avgS * eps * eps);
    }

    public override double Vega(double U)
    {
        double eps = 1e-4;
        double oldSig = sig;
        sig = oldSig * (1 + eps);
        double upPrice = Price();
        sig = oldSig * (1 - eps);
        double downPrice = Price();
        sig = oldSig;

        return (upPrice - downPrice) / (2 * oldSig * eps);
    }

    public override double Theta(double U)
    {
        double eps = 1e-4;
        double oldT = T;
        T = oldT + eps;
        double upPrice = Price();
        T = oldT - eps;
        double downPrice = Price();
        T = oldT;

        return (downPrice - upPrice) / (2 * eps);
    }

    public override double Rho(double U)
    {
        double eps = 1e-4;
        double oldR = r;
        r = oldR + eps;
        double upPrice = Price();
        r = oldR - eps;
        double downPrice = Price();
        r = oldR;

        return (upPrice - downPrice) / (2 * eps);
    }

    // ---------------------------------------------------------
    // Helper to compute payoff for perturbed paths
    // ---------------------------------------------------------
    private double PayoffForPath(double[] path)
    {
        double S_max = path.Max();
        double S_min = path.Min();

        if (otyp == "call")
            return Math.Max(S_max - K, 0.0);
        else
            return Math.Max(K - S_min, 0.0);
    }
}
