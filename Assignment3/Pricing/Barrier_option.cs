using System;
using System.Linq; // for Max/Min

public class Barrier_option : Option
{

    // Barrier properties
    public double Barrier;   // barrier level
    public string BarrierType; // "up-and-out", "down-and-out", "up-and-in", "down-and-in"

    public Barrier_option() : base()
    {
        Console.WriteLine("Enter barrier level: ");
        Barrier = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Enter barrier type (up-and-out / down-and-out / up-and-in / down-and-in): ");
        BarrierType = (Console.ReadLine() ?? "").Trim().ToLower();
    }

    // ---------------------------------------------------------
    // Check barrier condition helper
    // ---------------------------------------------------------
    private bool IsBarrierHit(double[] U)
    {
        double maxS = U.Max();
        double minS = U.Min();

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
    public override double Payoff(double[] U)
    {
        bool hit = IsBarrierHit(U);
        double ST = U[^1]; // terminal price

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
    public double Price(double[] U)
    {
        double ST = U[^1];
        double tmp = sig * Math.Sqrt(T);
        double d1 = (Math.Log(ST / K) + (b + 0.5 * sig * sig) * T) / tmp;
        double d2 = d1 - tmp;

        double vanillaPrice = otyp == "call"
            ? ST * Math.Exp((b - r) * T) * SpecialFunctions.N(d1) - K * Math.Exp(-r * T) * SpecialFunctions.N(d2)
            : K * Math.Exp(-r * T) * SpecialFunctions.N(-d2) - ST * Math.Exp((b - r) * T) * SpecialFunctions.N(-d1);

        // Apply simple barrier rule
        bool hit = IsBarrierHit(U);

        if (BarrierType.Contains("out"))
            return hit ? 0.0 : vanillaPrice;
        else if (BarrierType.Contains("in"))
            return hit ? vanillaPrice : 0.0;
        else
            return vanillaPrice;
    }

}
