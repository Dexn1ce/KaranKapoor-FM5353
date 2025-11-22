// Program.cs
using System;
using MonteCarloSim;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program_main
{

    static void Main()
    {
        Console.WriteLine("Please mention the type of the option:");
        string option_type = Console.ReadLine();
        Option baseopt;
        if (option_type == "asian")
        {
            baseopt = new Asian_option();
	    Asian_option opt = (Asian_option) baseopt;
        }
        else if (option_type == "digital")
        {
            baseopt = new Digital_option();
	    Digital_option opt = (Digital_option) baseopt;
	    //Console.WriteLine($"Black-Scholes price : {d.Price()}");
        }
        else if (option_type == "barrier")
        {
            baseopt = new Barrier_option();
	    Barrier_option opt = (Barrier_option) baseopt;

	    //Console.WriteLine($"Black-Scholes price : {opt.Price()}");
        }
        else if (option_type == "lookback")
        {
            baseopt = new Lookback_option();

	    Lookback_option opt = (Lookback_option) baseopt;

	    Console.WriteLine($"Black-Scholes price : {opt.Price()}");
        }
         else if (option_type == "range")
        {
            baseopt = new Range_option();
	    Range_option opt = (Range_option) baseopt;
        }
        else
        {
            Console.WriteLine("Invalid input. Defaulting to Vanilla type option");
            baseopt = new Option();
	    Option opt = baseopt;
        }


        Console.Write("Number of time steps (N): ");
        int N = int.TryParse(Console.ReadLine(), out var n) ? n : 1;
        Console.Write("Number of simulations (M): ");
        int M = int.TryParse(Console.ReadLine(), out var m) ? m : 100000;

        Console.WriteLine("Use antithetic? (true/false): ");
	bool useAnt = Convert.ToBoolean(Console.ReadLine());
        Console.WriteLine("Use Control Variate? (true/false): ");
	bool useControl = Convert.ToBoolean(Console.ReadLine());


	Console.WriteLine("Parallelization (true/false)");
        bool parallel = Convert.ToBoolean(Console.ReadLine());

	var type_SIM = VarianceReductionType.None;
        if (useAnt && !useControl && parallel)
        {
            Stopwatch sw = Stopwatch.StartNew();
	    type_SIM = VarianceReductionType.Antithetic;

	    Simulation sim = new Simulation(N,M,type_SIM, parallel);
            Console.WriteLine($"Detected cores: {Environment.ProcessorCount}. Parallel = {parallel}");


            var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);
	    sw.Stop();
            Console.WriteLine($"Price = {mean:F6}, StdErr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Parallel): {sw.ElapsedMilliseconds} ms");
        }
        else if (useAnt && !useControl && !parallel)
        {

            Stopwatch sw = Stopwatch.StartNew();

	    type_SIM = VarianceReductionType.Antithetic;

	    Simulation sim = new Simulation(N,M,type_SIM, parallel);





            Console.WriteLine($"Detected cores: {Environment.ProcessorCount}. Parallel = {parallel}");


            var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

            sw.Stop();
            Console.WriteLine($"Price = {mean:F6}, StdErr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");


        }

        else if (useAnt && useControl && !parallel)
        {
            Stopwatch sw = Stopwatch.StartNew();
            //var resultAnti = MonteCarloRunner.RunAntiControlParallel(sim, opt, N, M, parallel)
	    //

	    type_SIM = VarianceReductionType.AntiControl;

	    Simulation sim = new Simulation(N,M,type_SIM, parallel);


            var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);



            sw.Stop();
            Console.WriteLine($"price = {mean:F6}, stderr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");

        }

        else if (useAnt && useControl && parallel)
        {
            Stopwatch sw = Stopwatch.StartNew();
            //var resultAnti = MonteCarloRunner.RunAntiControlParallel(sim, opt, N, M, parallel)
	    //

	    type_SIM = VarianceReductionType.AntiControl;

	    Simulation sim = new Simulation(N,M,type_SIM, parallel);


            var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);



            sw.Stop();
            Console.WriteLine($"price = {mean:F6}, stderr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Parallel): {sw.ElapsedMilliseconds} ms");

        }
        else if (useControl && !useAnt && !parallel)
        {
            Stopwatch sw = Stopwatch.StartNew();


	    type_SIM = VarianceReductionType.ControlVariate;

	    Simulation sim = new Simulation(N,M,type_SIM, parallel);



            var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

            sw.Stop();
            Console.WriteLine($"price = {mean:F6}, stderr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");



        }

        else if (useControl && !useAnt && parallel)
        {
            Stopwatch sw = Stopwatch.StartNew();


	    type_SIM = VarianceReductionType.ControlVariate;

	    Simulation sim = new Simulation(N,M,type_SIM, parallel);



            var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

            sw.Stop();
            Console.WriteLine($"price = {mean:F6}, stderr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Parallel): {sw.ElapsedMilliseconds} ms");



        }
        else if (!useControl && !useAnt && !parallel)
        {
            Stopwatch sw = Stopwatch.StartNew();

	    type_SIM = VarianceReductionType.None;


	    Simulation sim = new Simulation(N,M,type_SIM, parallel);


            var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

            sw.Stop();
            Console.WriteLine($"Price = {mean:F6}, StdErr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Sequential): {sw.ElapsedMilliseconds} ms");

        }
        else if ( !useAnt && !useControl && parallel)
        {
            Stopwatch sw = Stopwatch.StartNew();

	    type_SIM= VarianceReductionType.None;

	    Simulation sim = new Simulation(N,M,type_SIM,parallel);

            Console.WriteLine($"Detected cores: {Environment.ProcessorCount}. Parallel = {parallel}");


            var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);
            sw.Stop();
            Console.WriteLine($"Price = {mean:F6}, StdErr = {stderr:F6}");
            Console.WriteLine($"Execution Time (Parallel): {sw.ElapsedMilliseconds} ms");

         
        }



    }
}
