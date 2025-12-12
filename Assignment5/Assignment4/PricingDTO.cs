namespace Assignment4;


using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class TobePricedOptionClass
{



    public string OptionType { get; set; } = "european";

    public double K { get; set; }

    public double S {get;set;}

    public double sig {get; set;}

    public double b {get; set;}

    public double r {get; set;}

    public double T {get; set;}

    public string? optiontyp {get; set;}

    public double? barrierlevel {get; set;} = 0;
    public string? barriertype {get; set;} = null;

    public double? payoutamount {get; set;}=0;

    public int N {get; set;}

    public int M {get; set;}

    public bool useAnt {get; set;}

   public bool useCont {get; set;}

  public bool isParallel {get; set;} 


		

}


public class GreeksResult
{
	public double Delta {get; set;}
	public double Vega {get; set;}
	public double Rho {get; set;}
	public double Theta {get; set;}
}


