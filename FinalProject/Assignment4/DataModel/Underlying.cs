
using System;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("underlying")]
public class underlying
{
	[Key]
	[Column("underlyingid")]
	public int underlyingid{get; set;}

	[Column("symbol")]	
	public string symbol{get;set;} = "";
	[Column("name")]
	public string name{get; set;} = ""; 
	[Column("assettype")]
	public string assettype{get; set;} ="";

	public ICollection<historicalPrice> prices {get; } = new List<historicalPrice>();

	public ICollection<Trade> Trades {get;} = new List<Trade>();

	public ICollection<Option> options {get; } = new List<Option>();
	public ICollection<Asian_option> asian_option {get; } = new List<Asian_option>();
	public ICollection<Barrier_option> barrier_option {get; } = new List<Barrier_option>();

	public ICollection<Range_option> range_option {get; } = new List<Range_option>();

	public ICollection<Lookback_option> lookback_option {get; } = new List<Lookback_option>();

	public ICollection<Digital_option> digital_option {get; } = new List<Digital_option>();

}
