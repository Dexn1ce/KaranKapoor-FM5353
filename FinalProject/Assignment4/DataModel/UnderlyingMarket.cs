
using System;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


[Table("UnderlyingMarket")]
public class UnderlyingMarket
{
	[Column("underlyingid")]	
	public int UnderlyingId {get; set;}
	public underlying? underlying {get; set;}
	[Column("marketid")]
	public int MarketId {get; set;}
	public Market? Market {get; set;}
}
