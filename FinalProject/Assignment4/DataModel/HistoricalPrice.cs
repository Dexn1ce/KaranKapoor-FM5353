
using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("historicalprice")]
public class historicalPrice
{
	[Key]
	[Column("historicalpriceid")]
	public int historicalpriceid {get; set;}
	public int underlyingid {get; set;}
	
	[JsonIgnore]
	[ForeignKey("underlyingid")]
	public underlying? Underlying {get; set;}
	[Column("pricetime")]
	public DateTime PriceTime {get; set;}
	[Column("lastprice")]
	public double LastPrice {get; set;} 
}
