using System;
using System.Linq;
using System.Threading;


namespace MonteCarloSim{

	internal class ThreadObject
	{
		public int StartIndex;
		public int count;
		public Option optInstance;
		public Simulation simInstance;
		public int seed;
		public double[] Results;

		public ThreadObject(int stInd,Simulation sim, int count_par, Option opt, double[] res, int seed_sim = 100)
		{
			StartIndex = stInd;
			count = count_par;
			optInstance = opt;
			simInstance = sim;
			Results = res;
			seed = seed_sim;
		}

	}

	internal class CVPilotAgg
	{
		public double sumX;
		public double sumY;
		public double sumXX;
		public double sumXY;
	}

	internal class sumAgg
	{
		public double beta;
		public double sum;
		public double sumsq;

		public sumAgg(double betaglobal, double suminit, double sumsqinit)
		{
			beta = betaglobal;
			sum = suminit;
			sumsq = sumsqinit;
		}
	}

	
	public class MonteCarloRunner
	{
		public static void ThreadWorker(object? obj)
		{
			var p = (ThreadObject) obj!;
			Random z = new Random(p.seed);

			for(int i=0; i< p.count; i++)
			{
				int globalStartIndex = p.StartIndex + i;
				double[] path = new double[p.simInstance.N];
				path[0] = p.optInstance.S;
				double sample = p.simInstance.Sample(p.optInstance, z,path, p.simInstance.type);
				p.Results[globalStartIndex] = sample;
			}

		}
		
		public static void ThreadWorkerAgg(object? obj)
		{
			var p = ((ThreadObject thread, CVPilotAgg agg)) obj!;
			Random z = new Random(p.thread.seed);

			for(int i=0; i<p.thread.count; i++)
			{
				int globalStartIndex = p.thread.StartIndex +i;
				double[] path = new double[p.thread.simInstance.N];
				path[0] = p.thread.optInstance.S;
				double Y = p.thread.simInstance.Sample(p.thread.optInstance, z, path, p.thread.simInstance.type);
				double X = path[^1];
				p.agg.sumX += X;
				p.agg.sumY += Y;
				p.agg.sumXX += X*X;
				p.agg.sumXY += X*Y;
			}
		}
		public static void ThreadWorkerFinal(object? obj)
		{
			var p = ((ThreadObject thread,sumAgg agg)) obj!;
		       	Random z = new Random(p.thread.seed);
			double EX = p.thread.optInstance.S * Math.Exp(p.thread.optInstance.r * p.thread.optInstance.T);
			
			for(int i=0; i<p.thread.count; i++)
			{

				int globalStartIndex = p.thread.StartIndex +i;
				double[] path = new double[p.thread.simInstance.N];
				path[0] = p.thread.optInstance.S;
				double Y = p.thread.simInstance.Sample(p.thread.optInstance, z, path, p.thread.simInstance.type);
				double X = path[^1];
				double Yadj = Y - p.agg.beta*(X -EX);
				p.agg.sum += Yadj;
				p.agg.sumsq += Yadj * Yadj;
				
			}	
		}	

	


		public static (double mean, double stderr) Run(Option opt, Simulation sim, bool isPar,VarianceReductionType type_sim,  int seed = 100)
		{
			if(!isPar)
			{
				Random z = new Random(seed);
				return sim.Run(opt,z,type_sim);
			}

			var results = new double[sim.M];
			double sum = 0.0, sum2 = 0.0;

			int cores = Math.Max(1, Environment.ProcessorCount);
			int threads = Math.Min(cores,sim.M);
			Thread[] tarr = new Thread[threads];

			int basecount =sim.M/threads;
			int remainder =sim.M % threads;
			int start = 0;
			int baseSeed = (Environment.TickCount & 0x7FFFFFFF);

			if(type_sim == VarianceReductionType.None || type_sim == VarianceReductionType.Antithetic)
			{

				ThreadObject[] parr = new ThreadObject[threads];

				for(int i=0; i< threads; i++)
				{
					int count = basecount + (i < remainder ? 1:0);
					int seed_new = baseSeed + i* 997;
					parr[i] = new ThreadObject(start,sim, count, opt, results, seed_new);
					tarr[i] = new Thread(new ParameterizedThreadStart(ThreadWorker));
					tarr[i].Start(parr[i]);
					start += count;
				}

				for(int i=0; i<threads; i++) tarr[i].Join();

				//aggreagting results: Simple , Antithetic


				for(int i=0; i<sim.M;i++)
	
				{
					sum += results[i];
					sum2 += results[i]*results[i];
				}



			}
			else if (type_sim == VarianceReductionType.ControlVariate|| type_sim == VarianceReductionType.AntiControl)
			{
				(ThreadObject thread, CVPilotAgg agg)[] arr;
				arr = new (ThreadObject, CVPilotAgg)[threads];	


				for(int i=0; i<threads; i++)
				{
					int count = basecount + (i < remainder ? 1:0);
					int threadStart = start;
					int threadCount = count;
					int seed_new = baseSeed + i*7919;

					arr[i] = (new ThreadObject(start, sim, count, opt, results, seed_new), new CVPilotAgg());
					tarr[i] = new Thread( new ParameterizedThreadStart(ThreadWorkerAgg));
					tarr[i].Start(arr[i]);
					start += count;
				}

				for(int i=0; i<threads; i++) tarr[i].Join();

				double total_sumX = 0.0, total_sumY = 0.0, total_sumXY = 0.0, total_sumXX = 0.0;

				for (int i=0; i<threads; i++)
				{
					total_sumX += arr[i].agg.sumX;
					total_sumY += arr[i].agg.sumY;
					total_sumXY += arr[i].agg.sumXY;
					total_sumXX += arr[i].agg.sumXX;

				}

				double cov = (total_sumXY / sim.M) - (total_sumX / sim.M) * (total_sumY / sim.M);
				double varX = Math.Max((total_sumXX / sim.M) - Math.Pow(total_sumX / sim.M, 2), 1e-14);
				double beta = cov / varX;


				//pass 2: compute adusted payoff in parallel
				
				(ThreadObject thread,sumAgg aggsum)[] Finalarr;
				Finalarr = new (ThreadObject, sumAgg)[threads];

				for(int i=0; i<threads; i++)
				{

					int count = basecount + (i < remainder ? 1:0);
					int threadStart = start;
					int threadCount = count;
					int seed_new = baseSeed +100000+ i*7919;

					Finalarr[i] = (new ThreadObject(start, sim, count, opt, results, seed_new),  new sumAgg(beta,0.0, 0.0));
					tarr[i] = new Thread(new ParameterizedThreadStart(ThreadWorkerFinal));
					tarr[i].Start(Finalarr[i]);
					start += count;
				}		

				for(int i=0; i<threads; i++) tarr[i].Join();


				for(int i=0; i<threads;i++)
				{
					sum += Finalarr[i].aggsum.sum;
					sum2 += Finalarr[i].aggsum.sumsq;
				}

			}


				double mean = sum/sim.M;
				double Var = Math.Max(sum2/(sim.M-1) - mean*mean,0);
				double stderr = Math.Sqrt(Var/sim.M);

				return (mean,stderr);
	}

}}




