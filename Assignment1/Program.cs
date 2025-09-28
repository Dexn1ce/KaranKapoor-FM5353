using System;
using MonteCarloSimulator;



public class Test_Sim
{
    static void Main()  
    {
    // Build option (yours prompts inside Option() are fine)
        Option opt = new Option();

        // Read inputs safely
        Console.Write("Please specify number of time steps (N): ");
        int N = int.TryParse(Console.ReadLine(), out var nTmp) ? nTmp : 1;

        Console.Write("Please specify number of simulation paths (M): ");
        int M = int.TryParse(Console.ReadLine(), out var mTmp) ? mTmp : 100000;

        Console.Write("Use Antithetic variates? (yes/no): ");
        string antStr = (Console.ReadLine() ?? "").Trim();
        bool useAnt = antStr.Length > 0 && antStr[0] is 'y' or 'Y';   // accepts y/yes

        Console.Write("Use Control variate? (yes/no): ");
        string cvStr = (Console.ReadLine() ?? "").Trim();
        bool useCV = cvStr.Length > 0 && cvStr[0] is 'y' or 'Y';


        // Choose the simulator
        // Assume all four classes share a common base 'Sim' and each has a Run(Option) method

        if (!useAnt && !useCV)
        {
            PlainSim sim = new PlainSim(N, M);
            (double mean, double stderr) = sim.Run(opt);
            Console.WriteLine($"Price of the option = {mean:F6}");
            Console.WriteLine($"Standard error      = {stderr:F6}");
        }
        else if (useAnt && !useCV)
        {
            AntitheticSim sim = new AntitheticSim(N, M);
            (double mean, double stderr) = sim.Run(opt);
            Console.WriteLine($"Price of the option = {mean:F6}");
            Console.WriteLine($"Standard error      = {stderr:F6}");
        }
        else if (!useAnt && useCV)
        {
            ControlVariate sim = new ControlVariate(N, M);

            (double mean, double stderr) = sim.Run(opt);
            Console.WriteLine($"Price of the option = {mean:F6}");
            Console.WriteLine($"Standard error      = {stderr:F6}");
        }
        else // useAnt && useCV
        {
            // You need a combined class (e.g., AntiControlSim) that does Antithetic + Control Variate
            AntiControlSim sim = new AntiControlSim(N, M);

            (double mean, double stderr) = sim.Run(opt, N,M);
            Console.WriteLine($"Price of the option = {mean:F6}");
            Console.WriteLine($"Standard error      = {stderr:F6}");
        }

        // Run

}

}