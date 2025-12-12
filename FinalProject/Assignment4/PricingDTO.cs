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

    public DateTime T {get; set;}

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

public class MCsim
{

    public int N {get; set;}

    public int M {get; set;}

    public DateTime simDate {get; set;}

    public bool useAnt {get; set;}

   public bool useCont {get; set;}

  public bool isParallel {get; set;} 

}

public class UnderlyingCreateDto
{
	[Required]
	public string Symbol {get; set;} = "";
	[Required]
	public string Name {get; set;} = "";
	[Required]
	public string AssetType {get; set;} = ""; 
}

public class HistoricalPriceUpdateDto
{
    public DateTime PriceTime { get; set; }
    public double LastPrice { get; set; }
}



public class ExchangeDTO
{
	
	public int id {get; set;}
	
	public string Name{get; set;} ="";
	public string Country {get; set;} ="";

	public DateTime CreatedDate {get; set;}
	
	public DateTime UpdatedDate {get; set;}	


}

public class MarketDTO
{
	public int exchangeid {get; set;}

	public string Name {get; set;} ="";

	public string Type {get; set;} = "";
}

public class TradeCreateDTO
{
	public int underlyingid {get; set;}
	public int marketid {get; set;}

	public string direction {get; set;} ="";

	public double quantity {get; set;}

	public double tradeprice {get; set;}

	public DateTime tradetime {get; set;}


}

public class OptionCreateDto
{
	public double K {get; set;}

	public double sig {get; set;}

	public double b {get; set;}

	public DateTime T {get; set;}
	[Required]
	public int underlyingid {get; set;}

	public int? ratecurveid {get; set;}

	public string? optiontype {get; set;} = null;

	public double? barrierlevel {get; set;} = 0;

	public string? barriertype {get; set;} = null;

	public double? payoutamount {get; set;} = 0;


}

