using System;
using System.Linq; // for Max/Min

public class Barrier_option : Option
{
    // Number of underlying prices in the simulated path
    int N_prices;
    public double[] S_path;

    // Barrier properties
    public double Barrier;   // barrier level
    public string BarrierType; // "up-and-out", "down-and-out", "up-and-in", "down-and-in"

    public Barrier_option()
    {
        Console.WriteLine("Enter barrier level: ");
        Barrier = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Enter barrier type (up-and-out / down-and-out / up-and-in / down-and-in): ");
        BarrierType = (Console.ReadLine() ?? "").Trim().ToLower();

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
    // Check barrier condition helper
    // ---------------------------------------------------------
    private bool IsBarrierHit()
    {
        double maxS = S_path.Max();
        double minS = S_path.Min();

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
    public override double Payoff(double dummy = 0.0)
    {
        bool hit = IsBarrierHit();
        double ST = S_path.Last(); // terminal price

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
    public override double Price()
    {
        // For simplicity, use average or final spot as effective S
        double ST = S_path.Last();
        double tmp = sig * Math.Sqrt(T);
        double d1 = (Math.Log(ST / K) + (b + 0.5 * sig * sig) * T) / tmp;
        double d2 = d1 - tmp;

        double vanillaPrice = otyp == "call"
            ? ST * Math.Exp((b - r) * T) * SpecialFunctions.N(d1) - K * Math.Exp(-r * T) * SpecialFunctions.N(d2)
            : K * Math.Exp(-r * T) * SpecialFunctions.N(-d2) - ST * Math.Exp((b - r) * T) * SpecialFunctions.N(-d1);

        // Apply simple barrier rule
        bool hit = IsBarrierHit();

        if (BarrierType.Contains("out"))
            return hit ? 0.0 : vanillaPrice;
        else if (BarrierType.Contains("in"))
            return hit ? vanillaPrice : 0.0;
        else
            return vanillaPrice;
    }

    // ---------------------------------------------------------
    // Greeks (approximate using vanilla formula; could be refined with simulation)
    // ---------------------------------------------------------
    public override double Delta()
    {
        double tmp = sig * Math.Sqrt(T);
        double d1 = (Math.Log(S / K) + (b + 0.5 * sig * sig) * T) / tmp;
        double delta = otyp == "call"
            ? Math.Exp((b - r) * T) * SpecialFunctions.N(d1)
            : Math.Exp((b - r) * T) * (SpecialFunctions.N(d1) - 1.0);

        // Zero out if knocked-out
        return (BarrierType.Contains("out") && IsBarrierHit()) ? 0.0 : delta;
    }

    public override double Gamma()
    {
        double tmp = sig * Math.Sqrt(T);
        double d1 = (Math.Log(S / K) + (b + 0.5 * sig * sig) * T) / tmp;
        double gamma = (SpecialFunctions.n(d1) * Math.Exp((b - r) * T)) / (S * tmp);

        return (BarrierType.Contains("out") && IsBarrierHit()) ? 0.0 : gamma;
    }

    public override double Vega(double U)
    {
        double tmp = sig * Math.Sqrt(T);
        double d1 = (Math.Log(S / K) + (b + 0.5 * sig * sig) * T) / tmp;
        double vega = U * Math.Exp((b - r) * T) * SpecialFunctions.n(d1) * Math.Sqrt(T);

        return (BarrierType.Contains("out") && IsBarrierHit()) ? 0.0 : vega;
    }

    public override double Theta(double U)
    {
        double tmp = sig * Math.Sqrt(T);
        double d1 = (Math.Log(U / K) + (b + (sig * sig) * 0.5) * T) / tmp;
        double d2 = d1 - tmp;

        double t1 = (S * Math.Exp((b - r) * T) * SpecialFunctions.n(d1) * sig * 0.5) / Math.Sqrt(T);
        double t2 = (b - r) * (U * Math.Exp((b - r) * T)) * SpecialFunctions.N(d1);
        double t3 = r * K * Math.Exp(-r * T) * SpecialFunctions.N(d2);

        double theta = -(t1 + t2 + t3);
        return (BarrierType.Contains("out") && IsBarrierHit()) ? 0.0 : theta;
    }

    public override double Rho(double U)
    {
        double tmp = sig * Math.Sqrt(T);
        double d1 = (Math.Log(U / K) + (b + (sig * sig) * 0.5) * T) / tmp;
        double d2 = d1 - tmp;

        double rho = otyp == "call"
            ? T * K * Math.Exp(-r * T) * SpecialFunctions.N(d2)
            : -T * K * Math.Exp(-r * T) * SpecialFunctions.N(-d2);

        return (BarrierType.Contains("out") && IsBarrierHit()) ? 0.0 : rho;
    }
}
