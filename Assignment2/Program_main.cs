// Program.cs
using System;
using MonteCarloSimulator;
using System.Diagnostics;

class Program
{
    static bool PromptYesNo(string prompt)
    {
        Console.Write(prompt);
        var r = (Console.ReadLine() ?? "").Trim();
        return r.Length > 0 && (r[0] == 'y' || r[0] == 'Y');
    }

    static void Main()
    {
        Option opt = new Option(); 

        Console.Write("Number of time steps (N): ");
        int N = int.TryParse(Console.ReadLine(), out var n) ? n : 1;
        Console.Write("Number of simulations (M): ");
        int M = int.TryParse(Console.ReadLine(), out var m) ? m : 100000;

        bool useAnt = PromptYesNo("Use antithetic? (y/n): ");
        bool useControl = PromptYesNo("Use Control Variate? (y/n): ");
        bool parallel = PromptYesNo("Enable parallelization? (y/n): ");

        //Sim sim;
        if (useAnt && !useControl && parallel)
        {
            AntitheticSim sim = new AntitheticSim(N, M, parallel);
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Detected cores: {Environment.ProcessorCount}. Parallel = {parallel}");
            sw.Stop();
            var (mean, stderr) = MonteCarloRunner.Run(sim, opt, parallel);
            Console.WriteLine($"Price = {mean:F6}, StdErr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");
        }
        else if (useAnt && !useControl && !parallel)
        {
            AntitheticSim sim = new AntitheticSim(N, M, parallel);

            Stopwatch sw = Stopwatch.StartNew();


            Console.WriteLine($"Detected cores: {Environment.ProcessorCount}. Parallel = {parallel}");

            var (mean, stderr) = MonteCarloRunner.Run(sim, opt, parallel);
            sw.Stop();
            Console.WriteLine($"Price = {mean:F6}, StdErr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");


        }

        else if (useAnt && useControl && !parallel)
        {
            AntiControlSim sim = new AntiControlSim(N, M, false, null);
            Random rng = new Random();
            Stopwatch sw = Stopwatch.StartNew();
            //var resultAnti = MonteCarloRunner.RunAntiControlParallel(sim, opt, N, M, parallel);
            (double resultMean, double stderr) = sim.Run(opt, N, M, rng);
            sw.Stop();
            Console.WriteLine($"AntiControl price = {resultMean:F6}, stderr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");
        }
        else if (useAnt && useControl && parallel)
        {
            AntiControlSim sim = new AntiControlSim(N, M, false, null);

            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Detected cores: {Environment.ProcessorCount}. Parallel = {parallel}");

            var resultAnti = MonteCarloRunner.RunAntiControlParallel(sim, opt, N, M, parallel);
            sw.Stop();
            Console.WriteLine($"AntiControl price = {resultAnti.mean:F6}, stderr = {resultAnti.stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");
        }
        else if (useControl && parallel)
        {
            Stopwatch sw = Stopwatch.StartNew();
            
            Console.WriteLine($"Detected cores: {Environment.ProcessorCount}. Parallel = {parallel}");

            var resultCV = MonteCarloRunner.RunControlVariateParallel(opt, N, M, parallel);
            sw.Stop();
            Console.WriteLine($"CV price = {resultCV.mean:F6}, stderr = {resultCV.stderr:F6}");
            Console.WriteLine($"Execution Time (Parallel): {sw.ElapsedMilliseconds} ms");

        }
        else if (useControl && !parallel)
        {
            Stopwatch sw = Stopwatch.StartNew();
            PlainSim sim = new PlainSim(N, M, false, null);
            //var resultCV = MonteCarloRunner.RunControlVariateParallel(opt, N, M, parallel);
            Random rng = new Random();
            (double resultMean, double stderr) = sim.Run(opt,rng);

            sw.Stop();
            Console.WriteLine($"CV price = {resultMean:F6}, stderr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");



        }
        else if (!useControl && !useAnt && !parallel)
        {
            PlainSim sim = new PlainSim(N, M, parallel);
            Stopwatch sw = Stopwatch.StartNew();

            Console.WriteLine($"Detected cores: {Environment.ProcessorCount}. Parallel = {parallel}");
            var (mean, stderr) = MonteCarloRunner.Run(sim, opt, parallel);
            sw.Stop();
            Console.WriteLine($"Price = {mean:F6}, StdErr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");

        }
        else if ( !useAnt && !useControl && parallel)
        {
            PlainSim sim = new PlainSim(N, M, parallel);
            Stopwatch sw = Stopwatch.StartNew();

            Console.WriteLine($"Detected cores: {Environment.ProcessorCount}. Parallel = {parallel}");
            var (mean, stderr) = MonteCarloRunner.Run(sim, opt, parallel);
            sw.Stop();
            Console.WriteLine($"Price = {mean:F6}, StdErr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Parallel): {sw.ElapsedMilliseconds} ms");

            
        }


    }
}
