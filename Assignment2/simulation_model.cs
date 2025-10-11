using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;



namespace MonteCarloSimulator
{
    public abstract class Sim
    {
        public int N;
        public int M;

        bool isParallel; // for parallel functionality

        protected readonly Random rng;
        double[,] results; // for the price paths


        // NEW: sample one discounted payoff (to be called sequentially or from a worker thread with its own RNG)
        // Derived classes must implement this.
        public abstract double Sample(Option opt, Random rng);


        public Sim(int Nsteps, int Msteps, bool isPar, int? seed = null)
        {
            N = Nsteps;
            M = Msteps;
            isParallel = isPar;
            results = new double[Nsteps, Msteps];



            rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        //public abstract void SimAntithetic(Option opt, isAntithetic);

        //public abstract void SimControlVariate(Option opt, this.isControlVariate);
        protected double Z(Random rng)
        {
            double u1 = 1.0 - rng.NextDouble();
            double u2 = 1.0 - rng.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }

        public (double mean, double stderr) Run(Option opt, Random? seqRng1 = null)
        {
            double s = 0.0, s2 = 0.0;
            for (int i = 0; i < M; i++)
            {
                double y = Sample(opt, rng);
                s += y;
                s2 += y * y;
            }
            double mean = s / M;
            double var = Math.Max(s2 / M - mean * mean, 0.0);
            return (mean, Math.Sqrt(var / M));
        }

    }

    public class AntitheticSim : Sim
    {
        public AntitheticSim(int Nsteps, int Msteps, bool isPar = false, int? seed = null)
            : base(Nsteps, Msteps, isPar, seed) { }

        public override double Sample(Option opt, Random rng)
        {
            double disc = Math.Exp(-opt.r * opt.T);
            double mu = (opt.r - 0.5 * opt.sig * opt.sig) * opt.T;
            double sigT = opt.sig * Math.Sqrt(opt.T);

            double z = Z(rng);
            double ST1 = opt.S * Math.Exp(mu + sigT * z);
            double ST2 = opt.S * Math.Exp(mu + sigT * -z);

            double pv = disc * 0.5 * (opt.Payoff(ST1) + opt.Payoff(ST2));
            return pv;
        }

        public (double mean, double stderr) Run(Option opt, Random rng)
        {
            double disc = Math.Exp(-opt.r * opt.T);
            double mu = (opt.r - 0.5 * opt.sig * opt.sig) * opt.T;
            double sigT = opt.sig * Math.Sqrt(opt.T);

            double sum = 0.0, sum2 = 0.0;

            for (int j = 0; j < M; j++)
            {
                double z = Z(rng);
                double ST1 = opt.S * Math.Exp(mu + sigT * z);
                double ST2 = opt.S * Math.Exp(mu + sigT * (-z));
                double pv = Sample(opt, rng);
                sum += pv;
                sum2 += pv * pv;
            }

            double mean = sum / M;
            double var = Math.Max(sum2 / M - mean * mean, 0.0);
            double stderr = Math.Sqrt(var / M);
            return (mean, stderr);
        }
    }


    public class PlainSim : Sim
    {
        public PlainSim(int Nsteps, int Msteps, bool isPar = false, int? seed = null)
            : base(Nsteps, Msteps, isPar, seed) { }

        public override double Sample(Option opt, Random rng)
        {
            double disc = Math.Exp(-opt.r * opt.T);
            double mu = (opt.r - 0.5 * opt.sig * opt.sig) * opt.T;
            double sigT = opt.sig * Math.Sqrt(opt.T);

            double z = Z(rng);
            double ST = opt.S * Math.Exp(mu + sigT * z);
            double pv = disc * opt.Payoff(ST);
            return pv;
        }

        public (double mean, double stderr) Run(Option opt, Random rng)
        {
            double disc = Math.Exp(-opt.r * opt.T);
            double mu = (opt.r - 0.5 * opt.sig * opt.sig) * opt.T;
            double sigT = opt.sig * Math.Sqrt(opt.T);

            double sum = 0.0, sum2 = 0.0;

            for (int j = 0; j < M; j++)
            {
                double pv = this.Sample(opt, rng);
                sum += pv;
                sum2 += pv * pv; // <-- square the PV
            }

            double mean = sum / M;
            double var = Math.Max(sum2 / M - mean * mean, 0.0); // population variance of PV
            double stderr = Math.Sqrt(var / M);                 // std error of the mean
            return (mean, stderr);
        }
    }

    public class AntiControlSim : Sim
    {
        public AntiControlSim(int Nsteps, int Msteps, bool isPar,int? seed = null) : base(Nsteps, Msteps,isPar ,seed) { }

        public override double Sample(Option opt, Random rng)
        {
            throw new NotImplementedException();
        }

        static double BSDelta(bool isCall, double S, double K, double r, double sigma, double tau)
        {
            if (tau <= 0.0 || sigma <= 0.0) return isCall ? (S > K ? 1.0 : 0.0) : (S < K ? -1.0 : 0.0);
            double v = sigma * Math.Sqrt(tau);
            double d1 = (Math.Log(S / K) + (r + 0.5 * sigma * sigma) * tau) / v;
            return isCall ? SpecialFunctions.N(d1) : (SpecialFunctions.N(d1) - 1.0);
        }
        static (double Snext, double cInc) StepWithDeltaCV(bool isCall, double S, double K, double r, double sigma, double t, double T, double dt, double z)
        {
            double tau = Math.Max(1e-12, T - t);                 // remaining time
            double delta = BSDelta(isCall, S, K, r, sigma, tau); // BS delta at start of step
            double mu = (r - 0.5 * sigma * sigma) * dt;
            double sig = sigma * Math.Sqrt(dt);
            double Snext = S * Math.Exp(mu + sig * z);
            // Control increment: Δ * (S_{t+dt} - E[S_{t+dt}|t])
            double cInc = delta * (Snext - S * Math.Exp(r * dt));
            return (Snext, cInc);
        }
        
        public (double mean, double stderr) Run(Option opt, int N, int M, Random rng)
        {
            double T = opt.T, dt = T / N, disc = Math.Exp(-opt.r * T);
            bool isCall = (opt.otyp == "call");

            // Pass 1: estimate beta
            double sumY = 0, sumC = 0, sumCC = 0, sumYC = 0;
            for (int j = 0; j < M; j++)
            {
                double S1 = opt.S, S2 = opt.S, t = 0.0, C1 = 0.0, C2 = 0.0;
                for (int i = 0; i < N; i++, t += dt)
                {
                    double z = Z(rng);
                    var (Sn1, c1) = StepWithDeltaCV(isCall, S1, opt.K, opt.r, opt.sig, t, T, dt, z);
                    var (Sn2, c2) = StepWithDeltaCV(isCall, S2, opt.K, opt.r, opt.sig, t, T, dt, -z);
                    S1 = Sn1; S2 = Sn2; C1 += c1; C2 += c2;
                }
                double Y = disc * 0.5 * (opt.Payoff(S1) + opt.Payoff(S2));
                double C = 0.5 * (C1 + C2);
                sumY += Y; sumC += C; sumCC += C * C; sumYC += Y * C;
            }
            double cov = (sumYC / M) - (sumY / M) * (sumC / M);
            double varC = Math.Max((sumCC / M) - Math.Pow(sumC / M, 2), 1e-16);
            double beta = cov / varC;                  // or set to 1.0

            // Pass 2: adjusted estimator
            double s = 0.0, s2 = 0.0;
            for (int j = 0; j < M; j++)
            {
                double S1 = opt.S, S2 = opt.S, t = 0.0, C1 = 0.0, C2 = 0.0;
                for (int i = 0; i < N; i++, t += dt)
                {
                    double z = Z(rng);
                    var (Sn1, c1) = StepWithDeltaCV(isCall, S1, opt.K, opt.r, opt.sig, t, T, dt, z);
                    var (Sn2, c2) = StepWithDeltaCV(isCall, S2, opt.K, opt.r, opt.sig, t, T, dt, -z);
                    S1 = Sn1; S2 = Sn2; C1 += c1; C2 += c2;
                }
                double Y = disc * 0.5 * (opt.Payoff(S1) + opt.Payoff(S2));
                double C = 0.5 * (C1 + C2);
                double Yadj = Y - beta * C;
                s += Yadj; s2 += Yadj * Yadj;
            }
            double m = s / M;
            double var = Math.Max(s2 / M - m * m, 0.0);
            return (m, Math.Sqrt(var / M));
        }

    }



}
    

