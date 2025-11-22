namespace Assignment4.Services;
using Assignment4;
using MonteCarloSim;

public class MCPricer
{
	public (double mean, double stderr) PriceOption(TobePricedOptionClass opt)
	{

		string option_type = opt.optiontyp;
		Option baseopt;
		if (option_type == "asian")
		{
		    baseopt = new Asian_option(opt.S, opt.sig, opt.r,opt.b, opt.T,option_type, opt.K);
		}
		else if (option_type == "digital")
		{
		    baseopt = new Digital_option(opt.S, opt.sig, opt.r,opt.b, opt.T,option_type,opt.K,opt.payoutamount?? 1.0);
		}
		else if (option_type == "barrier")
		{
		    baseopt = new Barrier_option(opt.S, opt.sig, opt.r,opt.b, opt.T,option_type,opt.K, opt.barrierlevel??0.0, opt.barriertype??null);

		}
		else if (option_type == "lookback")
		{
		    baseopt = new Lookback_option(opt.S, opt.sig, opt.r,opt.b, opt.T,option_type,opt.K);


		}
		 else if (option_type == "range")
		{
		    baseopt = new Range_option(opt.S, opt.sig, opt.r,opt.b, opt.T);
		}
		else
		{
		    baseopt = new Option(opt.S, opt.sig, opt.r,opt.b, opt.T,option_type,opt.K);
		}


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
}
