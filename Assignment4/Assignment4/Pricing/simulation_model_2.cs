using System;
public enum VarianceReductionType
{
	None,
	Antithetic,
	ControlVariate,
	AntiControl
}


public class Simulation
{
	public int N;
	public int M;
	public bool isPar;
	public VarianceReductionType type = VarianceReductionType.None;



	public Simulation(int Nsteps, int Msims,VarianceReductionType type2, bool isParsim = false)
	{
		N = Nsteps;
		M = Msims;
		isPar = isParsim;
		type = type2;
	}


	public double Z(Random rng)
	{
		double u1 = 1.0 - rng.NextDouble();
		double u2 = 1.0 - rng.NextDouble();

		return Math.Sqrt(-2.0*Math.Log(u1))*Math.Cos(2.0*Math.PI*u2);

	}



	public double Sample(Option opt, Random rng,double[] path, VarianceReductionType type_sim)
	{
		double disc = Math.Exp(-opt.r * opt.T);
		double dt = opt.T/N;
		double sigdt = Math.Sqrt(dt)* opt.sig;
		double mu = (opt.r - 0.5*opt.sig*opt.sig)*dt;
		double pv = 0.0;

		if(type_sim == VarianceReductionType.Antithetic)
		{
			double[] path_anti = new double[N];
			path_anti[0] = opt.S;

	//		Console.WriteLine($"Smulation type {type_sim}");
			for(int i=1; i<N; i++)
			{
				double z = Z(rng);
				path[i] = path[i-1]*Math.Exp(mu + sigdt*z);
				path_anti[i] = path_anti[i-1]*Math.Exp(mu + sigdt*(-z));
				
			}
			pv = disc*0.5*(opt.Payoff(path) + opt.Payoff(path_anti));

			return pv;

			
		}
		else if (type_sim == VarianceReductionType.AntiControl)
		{
			double[] path_anti = new double[N];
			path_anti[0] = opt.S;
			//Console.WriteLine($"Smulation type {type_sim}");

			for(int i=1; i<N; i++)
			{
				double z = Z(rng);
				path[i] = path[i-1]* Math.Exp(mu + sigdt*(z));
				path_anti[i] = path_anti[i-1]* Math.Exp(mu + sigdt*(-z));
			}

			pv = disc*0.5*(opt.Payoff(path) + opt.Payoff(path_anti));

			path[^1] = 0.5*(path[^1] + path_anti[^1]); // this is specifically done for Antithetic Control variate

			return pv;
		}
		
	//	Console.WriteLine($"Smulation type {type_sim}");
		for(int i=1; i< N; i++)
		{
			double z = Z(rng);
			path[i] = path[i-1]*Math.Exp(mu + sigdt*z);
		//	Console.WriteLine("{0} - {1}",i,path[i]);
		}
		pv = disc*opt.Payoff(path);
		return pv;

	//Console.WriteLine($"Option's price according to black_scholes : {opt.Price(path)}");


	}


	public (double mean, double stderr) Run(Option opt, Random rng, VarianceReductionType type_sim)
	{
		double sum = 0.0, sum2 = 0.0;
		Console.WriteLine("The simulation is running!");
		Console.WriteLine($"Running {M} simulations with {N} steps");

		if(type_sim != VarianceReductionType.ControlVariate && type_sim != VarianceReductionType.AntiControl)
		{

			for(int j=0; j<M; j++)
			{
				double[] path = new double[N];
				path[0] = opt.S;
				//Console.WriteLine($"Option type : {opt.otyp}");
				double pv = Sample(opt, rng,path,type_sim);
				//Console.WriteLine(pv);
				sum +=pv;
				sum2 += pv*pv;

			}

		}
		else if (type_sim == VarianceReductionType.ControlVariate)
		{
			// First Pass
			
			double sumX = 0.0, sumY = 0.0, sumXX = 0.0, sumXY = 0.0;
			double EX = opt.S*Math.Exp(opt.r * opt.T);

			for(int j=0; j<M; j++)
			{
				double[] path = new double[N];
				path[0] = opt.S;
				double Y = Sample(opt, rng,path, type_sim);
				sumY += Y;
				double X = path[^1];
				sumX += X;
				sumXX += X*X;
				sumXY += X*Y;
			}
			double cv = (sumXY/M) - (sumX/M)*(sumY/M);
			double vx = Math.Max((sumXX/M) - Math.Pow(sumX/M,2),1e-14);
			double beta = cv/vx;

			// Second Pass	

			for(int j=0; j<M; j++)
			{
				double[] path = new double[N];
				path[0] = opt.S;
				double Y = Sample(opt, rng,path, type_sim);
				double X = path[^1];
				double Yadj = Y - beta*(X - EX);
				sum += Yadj; sum2 += Yadj*Yadj;
			}

		}
		else if (type_sim == VarianceReductionType.AntiControl)
		{
			// First Pass
			double sumX = 0.0, sumY = 0.0, sumXX = 0.0, sumXY = 0.0;
			double EX = opt.S*Math.Exp(opt.r * opt.T);
			
		       for(int j=0; j<M; j++)
		       {
			       double[] path = new double[N];
			       path[0] = opt.S;
			       double Y = Sample(opt, rng, path, type_sim);
			       sumY += Y;
			       double X = path[^1];
			       sumX += X;
			       sumXX += X*X;
			       sumXY += X*Y;
		       }

			double cv = (sumXY/M) - (sumX/M)*(sumY/M);
			double vx = Math.Max((sumXX/M) - Math.Pow(sumX/M,2), 1e-14);

			double beta = cv/vx;

			// Second Pass

			for(int j=0; j<M; j++)
			{
				double[] path = new double[N];
				path[0] = opt.S;
				double Y = Sample(opt, rng, path, type_sim);
				double X = path[^1];
				double Yadj = Y - beta*(X- EX);
				sum += Yadj; sum2 += Yadj*Yadj;

			}	
		}


			double mean = sum/M;
			double Var = Math.Max((sum2/(M-1)) - mean*mean,0);
			double stderr = Math.Sqrt(Var/M);

		
			return (mean, stderr);	



	}

}

