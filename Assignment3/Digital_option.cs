using System;

public class Digital_option : Option
{
    public Digital_option() 
    {
    }

    // ---------------------------------------------------------
    // Payoff
    // ---------------------------------------------------------
    public override double Payoff(double ST = 0.0)
    {
        if (otyp == "call")
            return (ST > K) ? 1.0 : 0.0;
        else
            return (ST < K) ? 1.0 : 0.0;
    }

    // ---------------------------------------------------------
    // Analytical Black-Scholes price for Digital Option
    // ---------------------------------------------------------
    public override double Price()
    {
        double tmp = sig * Math.Sqrt(T);
        double d2 = (Math.Log(S / K) + (b - 0.5 * sig * sig) * T) / tmp;

        if (otyp == "call")
            return Math.Exp(-r * T) * SpecialFunctions.N(d2);
        else
            return Math.Exp(-r * T) * SpecialFunctions.N(-d2);
    }

    // ---------------------------------------------------------
    // Greeks for Digital Option
    // ---------------------------------------------------------
    public override double Delta()
    {
        double tmp = sig * Math.Sqrt(T);
        double d2 = (Math.Log(S / K) + (b - 0.5 * sig * sig) * T) / tmp;

        // For Digital options, Delta is the PDF of d2 * sensitivity
        if (otyp == "call")
            return (Math.Exp((b - r) * T) * SpecialFunctions.n(d2)) / (S * sig * Math.Sqrt(T));
        else
            return -(Math.Exp((b - r) * T) * SpecialFunctions.n(d2)) / (S * sig * Math.Sqrt(T));
    }

    public override double Gamma()
    {
        double tmp = sig * Math.Sqrt(T);
        double d2 = (Math.Log(S / K) + (b - 0.5 * sig * sig) * T) / tmp;
        double n_d2 = SpecialFunctions.n(d2);

        // Gamma involves derivative of delta
        double factor = Math.Exp((b - r) * T) / (S * S * sig * sig * T);
        double part = -d2 * n_d2 / Math.Sqrt(T);
        return factor * part;
    }

    public override double Vega(double U)
    {
        double tmp = sig * Math.Sqrt(T);
        double d2 = (Math.Log(S / K) + (b - 0.5 * sig * sig) * T) / tmp;

        // Vega = ∂Price/∂σ
        if (otyp == "call")
            return -Math.Exp(-r * T) * SpecialFunctions.n(d2) * d2 * Math.Sqrt(T);
        else
            return Math.Exp(-r * T) * SpecialFunctions.n(d2) * d2 * Math.Sqrt(T);
    }

    public override double Theta(double U)
    {
        double tmp = sig * Math.Sqrt(T);
        double d2 = (Math.Log(S / K) + (b - 0.5 * sig * sig) * T) / tmp;
        double n_d2 = SpecialFunctions.n(d2);

        double term1 = -Math.Exp(-r * T) * n_d2 * ((b - r - 0.5 * sig * sig) / (sig * Math.Sqrt(T)));
        double term2 = r * Math.Exp(-r * T) * SpecialFunctions.N(d2);

        if (otyp == "call")
            return term1 - term2;
        else
            return -term1 - r * Math.Exp(-r * T) * SpecialFunctions.N(-d2);
    }

    public override double Rho(double U)
    {
        double tmp = sig * Math.Sqrt(T);
        double d2 = (Math.Log(S / K) + (b - 0.5 * sig * sig) * T) / tmp;

        if (otyp == "call")
            return -T * Math.Exp(-r * T) * SpecialFunctions.N(d2);
        else
            return -T * Math.Exp(-r * T) * SpecialFunctions.N(-d2);
    }
}
