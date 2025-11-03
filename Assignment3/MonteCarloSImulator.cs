// File: MonteCarloRunner.cs
using System;
using System.Threading;

namespace MonteCarloSimulator
{
    // single RunnerThreadParams used by simple Run(...)
    internal class RunnerThreadParams
    {
        public int StartIndex;
        public int Count;
        public Sim SimInstance;
        public Option Opt;
        public double[] Results; // shared; threads write disjoint ranges
        public int Seed;

        public RunnerThreadParams(int start, int count, Sim sim, Option opt, double[] results, int seed)
        {
            StartIndex = start;
            Count = count;
            SimInstance = sim;
            Opt = opt;
            Results = results;
            Seed = seed;
        }
    }

    // per-thread pilot aggregator for CV / AntiControl pilot pass
    internal class CVPilotAgg
    {
        public double sumY;
        public double sumX;
        public double sumXX;
        public double sumXY;
    }

    // per-thread final aggregator
    internal class SumAgg
    {
        public double sum;
        public double sumsq;
    }

    public static class MonteCarloRunner
    {
        // Worker for simple Run: writes per-path payoff values into p.Results
        private static void ThreadWorker(object? obj)
        {
            var p = (RunnerThreadParams)obj!;
            var rng = new Random(p.Seed);

            for (int i = 0; i < p.Count; i++)
            {
                int globalIndex = p.StartIndex + i;
                double sample = p.SimInstance.Sample(p.Opt, rng);
                p.Results[globalIndex] = sample;
            }
        }

        // Core API: run a simulation either sequentially or with threads (simple Sampler-based sims)
        public static (double mean, double stderr) Run(Sim sim, Option opt, bool parallel)
        {
            int M = sim.M;

            if (!parallel)
            {
                // Use sim's sequential Run helper (which uses Sample with sim.seqRng)
                return sim.Run(opt);
            }

            var results = new double[M];

            int cores = Math.Max(1, Environment.ProcessorCount);
            int threads = Math.Min(cores, M);
            Thread[] tarr = new Thread[threads];
            RunnerThreadParams[] parr = new RunnerThreadParams[threads];

            int baseCount = M / threads;
            int remainder = M % threads;
            int start = 0;
            int baseSeed = (Environment.TickCount & 0x7FFFFFFF);

            for (int i = 0; i < threads; i++)
            {
                int count = baseCount + (i < remainder ? 1 : 0);
                int seed = baseSeed + i * 997;
                parr[i] = new RunnerThreadParams(start, count, sim, opt, results, seed);
                tarr[i] = new Thread(new ParameterizedThreadStart(ThreadWorker));
                tarr[i].Start(parr[i]);
                start += count;
            }

            for (int i = 0; i < threads; i++) tarr[i].Join();

            // aggregate results
            double sum = 0.0, sum2 = 0.0;
            for (int i = 0; i < M; i++)
            {
                double y = results[i];
                sum += y;
                sum2 += y * y;
            }
            double mean = sum / M;
            double var = Math.Max(sum2 / M - mean * mean, 0.0);
            double stderr = Math.Sqrt(var / M);
            return (mean, stderr);
        }

        // --- two-pass parallel control variate ---
        public static (double mean, double stderr) RunControlVariateParallel(Option opt, int Nsteps, int M, bool parallel)
        {
            // serial fallback (calls a straightforward serial algorithm)
            if (!parallel)
            {
                //First Pass : estimate beta = Cov(Y,X)/Var(X), with Y = discounted payoff
                double mu = (opt.r - 0.5 * opt.sig * opt.sig) * opt.T;
                double sigT = opt.sig * Math.Sqrt(opt.T);
                double disc = Math.Exp(-opt.r * opt.T);
                double EX = opt.S * Math.Exp(opt.r * opt.T);

                double sumY = 0, sumX = 0, sumXX = 0, sumXY = 0;
                var rngSerial = new Random(12345);
                for (int j = 0; j < M; j++)
                {
                    double z = Z_local(rngSerial);
                    double ST = opt.S * Math.Exp(mu + sigT * z);
                    double Y = disc * opt.Payoff(ST);
                    double X = ST;
                    sumY += Y; sumX += X; sumXX += X * X; sumXY += X * Y;
                }
                double cv = (sumXY / M) - (sumX / M) * (sumY / M);
                double vX = Math.Max((sumXX / M) - Math.Pow(sumX / M, 2), 1e-14);
                double Seqbeta = cv / vX;
                
                // Second pass : adjusted estimator Y* = Y - beta (X- E[X])
                double s = 0, s2 = 0;
                rngSerial = new Random(54321);
                for (int j = 0; j < M; j++)
                {
                    double z = Z_local(rngSerial);
                    double ST = opt.S * Math.Exp(mu + sigT * z);
                    double Y = disc * opt.Payoff(ST);
                    double X = ST;
                    double Yadj = Y - Seqbeta * (X - EX);
                    s += Yadj; s2 += Yadj * Yadj;
                }
                double m = s / M;
                double var = Math.Max(s2 / M - m * m, 0.0);
                return (m, Math.Sqrt(var / M));
            }

            int cores = Math.Max(1, Environment.ProcessorCount);
            int threads = Math.Min(cores, M);
            int baseCount = M / threads;
            int remainder = M % threads;

            CVPilotAgg[] pilot = new CVPilotAgg[threads];
            Thread[] tarr = new Thread[threads];
            int start = 0;
            int baseSeed = (Environment.TickCount & 0x7fffffff);

            // PASS 1: pilot sums
            for (int i = 0; i < threads; i++)
            {
                int count = baseCount + (i < remainder ? 1 : 0);
                pilot[i] = new CVPilotAgg();
                int threadStart = start;
                int threadCount = count;
                int seed = baseSeed + i * 7919;
                Thread t = new Thread(new ParameterizedThreadStart(obj =>
                {
                    var tup = (Tuple<int, int, int, CVPilotAgg>)obj!;
                    int cnt = tup.Item2;
                    int sd = tup.Item3;
                    var agg = tup.Item4;
                    var rng = new Random(sd);

                    double mu = (opt.r - 0.5 * opt.sig * opt.sig) * opt.T;
                    double sigT = opt.sig * Math.Sqrt(opt.T);
                    double disc = Math.Exp(-opt.r * opt.T);

                    double local_sumY = 0.0, local_sumX = 0.0, local_sumXX = 0.0, local_sumXY = 0.0;
                    for (int j = 0; j < cnt; j++)
                    {
                        double z = Z_local(rng);
                        double ST = opt.S * Math.Exp(mu + sigT * z);
                        double Y = disc * opt.Payoff(ST);
                        double X = ST;
                        local_sumY += Y;
                        local_sumX += X;
                        local_sumXX += X * X;
                        local_sumXY += X * Y;
                    }
                    agg.sumY = local_sumY;
                    agg.sumX = local_sumX;
                    agg.sumXX = local_sumXX;
                    agg.sumXY = local_sumXY;
                }));
                t.Start(Tuple.Create(threadStart, threadCount, seed, pilot[i]));
                tarr[i] = t;
                start += count;
            }

            for (int i = 0; i < threads; i++) tarr[i].Join();

            double total_sumY = 0, total_sumX = 0, total_sumXX = 0, total_sumXY = 0;
            for (int i = 0; i < threads; i++)
            {
                total_sumY += pilot[i].sumY;
                total_sumX += pilot[i].sumX;
                total_sumXX += pilot[i].sumXX;
                total_sumXY += pilot[i].sumXY;
            }

            double cov = (total_sumXY / M) - (total_sumX / M) * (total_sumY / M);
            double varX = Math.Max((total_sumXX / M) - Math.Pow(total_sumX / M, 2), 1e-14);
            double beta = cov / varX;

            // PASS 2: compute adjusted payoffs in parallel
            SumAgg[] finalAgg = new SumAgg[threads];
            tarr = new Thread[threads];
            start = 0;
            for (int i = 0; i < threads; i++)
            {
                int count = baseCount + (i < remainder ? 1 : 0);
                finalAgg[i] = new SumAgg();
                int threadStart = start;
                int threadCount = count;
                int seed = baseSeed + 100000 + i * 7907;
                Thread t = new Thread(new ParameterizedThreadStart(obj =>
                {
                    var tup = (Tuple<int, int, int, SumAgg>)obj!;
                    int cnt = tup.Item2;
                    int sd = tup.Item3;
                    var agg = tup.Item4;
                    var rng = new Random(sd);

                    double mu = (opt.r - 0.5 * opt.sig * opt.sig) * opt.T;
                    double sigT = opt.sig * Math.Sqrt(opt.T);
                    double disc = Math.Exp(-opt.r * opt.T);
                    double EX = opt.S * Math.Exp(opt.r * opt.T);

                    double local_sum = 0.0, local_sum2 = 0.0;
                    for (int j = 0; j < cnt; j++)
                    {
                        double z = Z_local(rng);
                        double ST = opt.S * Math.Exp(mu + sigT * z);
                        double Y = disc * opt.Payoff(ST);
                        double X = ST;
                        double Yadj = Y - beta * (X - EX);
                        local_sum += Yadj;
                        local_sum2 += Yadj * Yadj;
                    }
                    agg.sum = local_sum;
                    agg.sumsq = local_sum2;
                }));
                t.Start(Tuple.Create(threadStart, threadCount, seed, finalAgg[i]));
                tarr[i] = t;
                start += count;
            }

            for (int i = 0; i < threads; i++) tarr[i].Join();

            double total = 0.0, totalSq = 0.0;
            for (int i = 0; i < threads; i++)
            {
                total += finalAgg[i].sum;
                totalSq += finalAgg[i].sumsq;
            }

            double mean = total / M;
            double variance = Math.Max(totalSq / M - mean * mean, 0.0);
            double stderr = Math.Sqrt(variance / M);
            return (mean, stderr);
        }

        // --- parallel Antithetic + delta-control (AntiControl) ---
        public static (double mean, double stderr) RunAntiControlParallel(Sim sim, Option opt, int N, int M, bool parallel)
        {
            if (!parallel)
            {
                return sim.Run(opt); // assumes serial Run exists on AntiControlSim
            }

            int cores = Math.Max(1, Environment.ProcessorCount);
            int threads = Math.Min(cores, M);
            int baseCount = M / threads;
            int remainder = M % threads;

            CVPilotAgg[] pilot = new CVPilotAgg[threads];
            Thread[] tarr = new Thread[threads];
            int start = 0;
            int baseSeed = (Environment.TickCount & 0x7fffffff);

            double T = opt.T;
            double dt = T / Math.Max(1, N);
            bool isCall = opt.otyp == "call";

            // PASS 1: pilot (antithetic + delta-control accumulation)
            for (int i = 0; i < threads; i++)
            {
                int count = baseCount + (i < remainder ? 1 : 0);
                pilot[i] = new CVPilotAgg();
                int threadStart = start;
                int threadCount = count;
                int seed = baseSeed + i * 7919;
                Thread t = new Thread(new ParameterizedThreadStart(obj =>
                {
                    var tup = (Tuple<int, int, int, CVPilotAgg>)obj!;
                    int cnt = tup.Item2;
                    int sd = tup.Item3;
                    var agg = tup.Item4;
                    var rng = new Random(sd);

                    double local_sumY = 0.0, local_sumC = 0.0, local_sumCC = 0.0, local_sumYC = 0.0;

                    for (int j = 0; j < cnt; j++)
                    {
                        double S1 = opt.S, S2 = opt.S;
                        double ttime = 0.0;
                        double C1 = 0.0, C2 = 0.0;
                        for (int k = 0; k < N; k++, ttime += dt)
                        {
                            double z = Z_local(rng);
                            var r1 = StepWithDeltaCV_local(isCall, S1, opt.K, opt.r, opt.sig, ttime, T, dt, z);
                            var r2 = StepWithDeltaCV_local(isCall, S2, opt.K, opt.r, opt.sig, ttime, T, dt, -z);
                            S1 = r1.Snext; C1 += r1.cInc;
                            S2 = r2.Snext; C2 += r2.cInc;
                        }
                        double disc = Math.Exp(-opt.r * T);
                        double Y = disc * 0.5 * (opt.Payoff(S1) + opt.Payoff(S2));
                        double C = 0.5 * (C1 + C2);
                        local_sumY += Y;
                        local_sumC += C;
                        local_sumCC += C * C;
                        local_sumYC += Y * C;
                    }

                    agg.sumY = local_sumY;
                    agg.sumX = local_sumC;
                    agg.sumXX = local_sumCC;
                    agg.sumXY = local_sumYC;
                }));
                t.Start(Tuple.Create(threadStart, threadCount, seed, pilot[i]));
                tarr[i] = t;
                start += count;
            }

            for (int i = 0; i < threads; i++) tarr[i].Join();

            double totalY = 0, totalC = 0, totalCC = 0, totalYC = 0;
            for (int i = 0; i < threads; i++)
            {
                totalY += pilot[i].sumY;
                totalC += pilot[i].sumX;
                totalCC += pilot[i].sumXX;
                totalYC += pilot[i].sumXY;
            }

            double cov = (totalYC / M) - (totalY / M) * (totalC / M);
            double varC = Math.Max((totalCC / M) - Math.Pow(totalC / M, 2), 1e-16);
            double beta = cov / varC;

            // PASS 2: compute adjusted payoffs
            SumAgg[] finalAgg = new SumAgg[threads];
            tarr = new Thread[threads];
            start = 0;
            for (int i = 0; i < threads; i++)
            {
                int count = baseCount + (i < remainder ? 1 : 0);
                finalAgg[i] = new SumAgg();
                int threadStart = start;
                int threadCount = count;
                int seed = baseSeed + 100000 + i * 7907;
                Thread t = new Thread(new ParameterizedThreadStart(obj =>
                {
                    var tup = (Tuple<int, int, int, SumAgg>)obj!;
                    int cnt = tup.Item2;
                    int sd = tup.Item3;
                    var agg = tup.Item4;
                    var rng = new Random(sd);

                    double local_sum = 0.0, local_sum2 = 0.0;
                    for (int j = 0; j < cnt; j++)
                    {
                        double S1 = opt.S, S2 = opt.S;
                        double ttime = 0.0;
                        double C1 = 0.0, C2 = 0.0;
                        for (int k = 0; k < N; k++, ttime += dt)
                        {
                            double z = Z_local(rng);
                            var r1 = StepWithDeltaCV_local(isCall, S1, opt.K, opt.r, opt.sig, ttime, T, dt, z);
                            var r2 = StepWithDeltaCV_local(isCall, S2, opt.K, opt.r, opt.sig, ttime, T, dt, -z);
                            S1 = r1.Snext; C1 += r1.cInc;
                            S2 = r2.Snext; C2 += r2.cInc;
                        }
                        double disc = Math.Exp(-opt.r * T);
                        double Y = disc * 0.5 * (opt.Payoff(S1) + opt.Payoff(S2));
                        double C = 0.5 * (C1 + C2);
                        double Yadj = Y - beta * C;
                        local_sum += Yadj;
                        local_sum2 += Yadj * Yadj;
                    }
                    agg.sum = local_sum;
                    agg.sumsq = local_sum2;
                }));
                t.Start(Tuple.Create(threadStart, threadCount, seed, finalAgg[i]));
                tarr[i] = t;
                start += count;
            }

            for (int i = 0; i < threads; i++) tarr[i].Join();

            double total = 0.0, totalSq = 0.0;
            for (int i = 0; i < threads; i++)
            {
                total += finalAgg[i].sum;
                totalSq += finalAgg[i].sumsq;
            }

            double mean = total / M;
            double variance = Math.Max(totalSq / M - mean * mean, 0.0);
            double stderr = Math.Sqrt(variance / M);
            return (mean, stderr);
        }

        // small helpers
        private static double Z_local(Random rng)
        {
            double u1 = 1.0 - rng.NextDouble();
            double u2 = 1.0 - rng.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }

        private static (double Snext, double cInc) StepWithDeltaCV_local(bool isCall, double S, double K, double r, double sigma, double t, double T, double dt, double z)
        {
            double tau = Math.Max(1e-12, T - t);
            double delta = BSDelta_local(isCall, S, K, r, sigma, tau);
            double mu = (r - 0.5 * sigma * sigma) * dt;
            double sig = sigma * Math.Sqrt(dt);
            double Snext = S * Math.Exp(mu + sig * z);
            double cInc = delta * (Snext - S * Math.Exp(r * dt));
            return (Snext, cInc);
        }

        private static double BSDelta_local(bool isCall, double S, double K, double r, double sigma, double tau)
        {
            if (tau <= 0.0 || sigma <= 0.0) return isCall ? (S > K ? 1.0 : 0.0) : (S < K ? -1.0 : 0.0);
            double v = sigma * Math.Sqrt(tau);
            double d1 = (Math.Log(S / K) + (r + 0.5 * sigma * sigma) * tau) / v;
            return isCall ? SpecialFunctions.N(d1) : (SpecialFunctions.N(d1) - 1.0);
        }
    }
}
