namespace Assignment4.Services;
using Assignment4;
using MonteCarloSim;


public enum OptionType
{
	European,
	Asian,
	Digital,
	Lookback,
	Range,
	Barrier
}

public static class OptionFactory
{
	public static Option Create(TobePricedOptionClass opt,OptionType type)
	{
		return type switch
		{
			OptionType.Asian => new Asian_option(opt.S, opt.sig,opt.b, opt.r, opt.T, opt.optiontyp ?? throw new ArgumentNullException(nameof(opt.optiontyp)),opt.K),
			OptionType.Digital => new Digital_option(opt.S, opt.sig, opt.b, opt.r, opt.T, opt.optiontyp  ?? throw new ArgumentNullException(nameof(opt.optiontyp)) ,opt.K,opt.payoutamount ?? throw new ArgumentNullException(nameof(opt.payoutamount))),
			OptionType.Lookback => new Lookback_option(opt.S, opt.sig, opt.b, opt.r, opt.T,opt.optiontyp  ?? throw new ArgumentNullException(nameof(opt.optiontyp)), opt.K),
			OptionType.Range => new Range_option(opt.S, opt.sig, opt.b, opt.r, opt.T),
			OptionType.European => new Option(opt.S, opt.sig,opt.b, opt.r, opt.T, opt.optiontyp  ?? throw new ArgumentNullException(nameof(opt.optiontyp)) ,opt.K),
			OptionType.Barrier => new Barrier_option(opt.S, opt.sig,opt.b, opt.r, opt.T, opt.optiontyp  ?? throw new ArgumentNullException(nameof(opt.optiontyp)) ,opt.K, opt.barrierlevel ?? throw new ArgumentNullException(nameof(opt.barrierlevel)) , opt.barriertype ?? throw new ArgumentNullException(nameof(opt.barriertype))),
			_ => throw new ArgumentException("Unknown option type")
		};
	}
}



public class MCPricer
{



	const double hSpot = 0.01;     
	const double hVol  = 0.0001;   
	const double hRate = 0.0001;   
	const double hStrike = 0.01;   
	const double hTime = 1.0 / 365;

	private double Revalue(TobePricedOptionClass opt)
	{
		return PriceOption(opt).mean;
	}

	private void BumpSpot(TobePricedOptionClass o, double h) => o.S += h;
	private void BumpVol(TobePricedOptionClass o, double h)  => o.sig += h;
	private void BumpRate(TobePricedOptionClass o, double h) => o.r += h;
	private void BumpStrike(TobePricedOptionClass o, double h) => o.K += h;
	private void BumpTime(TobePricedOptionClass o, double h) => o.T += h;


	private double CentralDifference( TobePricedOptionClass opt, Action<TobePricedOptionClass, double> applyBump, double h)
	{
		applyBump(opt, h);
		double up = PriceOption(opt).mean;

		applyBump(opt, -2*h);
		double down = PriceOption(opt).mean;

		applyBump(opt, h);

		return (up - down)/(2*h);


	}

	static OptionType ParseOptionType(string input)
	{
		return input.ToLower() switch
		{
			"vanilla" => OptionType.European,
			"asian" => OptionType.Asian,
			"digital" => OptionType.Digital,
			"lookback" => OptionType.Lookback,
			"range" => OptionType.Range,
			"barrier" => OptionType.Barrier,
			_ => OptionType.European
		};
	}
	public (double mean, double stderr) PriceOption(TobePricedOptionClass opt)
	{

		OptionType optType = ParseOptionType(opt.OptionType);
		Option baseopt = OptionFactory.Create(opt, optType);


		int N =opt.N ;
		int M = opt.M;

		bool useAnt = opt.useAnt;
		bool useControl = opt.useCont;


		bool parallel = opt.isParallel;

		var type_SIM = VarianceReductionType.None;
		if (useAnt && !useControl && parallel)
		{
		    type_SIM = VarianceReductionType.Antithetic;

		    Simulation sim = new Simulation(N,M,type_SIM, parallel);


		    var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

		    return (mean, stderr);
		}
		else if (useAnt && !useControl && !parallel)
		{


		    type_SIM = VarianceReductionType.Antithetic;

		    Simulation sim = new Simulation(N,M,type_SIM, parallel);

		    var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

		    return (mean, stderr);

		}

		else if (useAnt && useControl && !parallel)
		{

		    type_SIM = VarianceReductionType.AntiControl;

		    Simulation sim = new Simulation(N,M,type_SIM, parallel);


		    var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

		    return (mean, stderr);



		}

		else if (useAnt && useControl && parallel)
		{

		    type_SIM = VarianceReductionType.AntiControl;

		    Simulation sim = new Simulation(N,M,type_SIM, parallel);


		    var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

		    return (mean, stderr);




		}
		else if (useControl && !useAnt && !parallel)
		{


		    type_SIM = VarianceReductionType.ControlVariate;

		    Simulation sim = new Simulation(N,M,type_SIM, parallel);



		    var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

		    return (mean, stderr);


		}

		else if (useControl && !useAnt && parallel)
		{


		    type_SIM = VarianceReductionType.ControlVariate;

		    Simulation sim = new Simulation(N,M,type_SIM, parallel);



		    var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

		    return (mean, stderr);


		}
		else if (!useControl && !useAnt && !parallel)
		{

		    type_SIM = VarianceReductionType.None;


		    Simulation sim = new Simulation(N,M,type_SIM, parallel);


		    var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

		    return (mean, stderr);
		}
		else if ( !useAnt && !useControl && parallel)
		{

		    type_SIM= VarianceReductionType.None;

		    Simulation sim = new Simulation(N,M,type_SIM,parallel);

		    var (mean, stderr) = MonteCarloRunner.Run( baseopt,sim,parallel, type_SIM);

		    return (mean, stderr);
		 
		}

		return (0,0);
	}

	public GreeksResult ComputeGreeks(TobePricedOptionClass opt)
	{
	    double delta = CentralDifference(opt, BumpSpot, 0.01);
	    double vega  = CentralDifference(opt,BumpVol, 0.0001);
	    double rho   = CentralDifference(opt,BumpRate, 0.0001);

	    double thetaH = 1.0 / 365.0;

	    double basePrice = Revalue(opt);
	    BumpTime(opt, -thetaH);
	    double bumped = Revalue(opt);
	    BumpTime(opt, thetaH);

	    double theta = (bumped - basePrice) / thetaH;

	    return new GreeksResult
	    {
		Delta = delta,
		Vega  = vega,
		Rho   = rho,
		Theta = theta
	    };
	}

}
