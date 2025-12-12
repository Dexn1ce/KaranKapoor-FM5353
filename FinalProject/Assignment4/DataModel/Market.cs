
using System;


using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("market")]
public class Market
{
	[Key]
	[Column("marketid")]
	public int id {get; set;}
	[Column("exchangeid")]
	public int Exchangeid {get; set;}
	[JsonIgnore]	
	public Exchange? exchange {get; set;}

	[Column("name")]	
	public string name {get; set;} ="";

	[Column("type")]	
	public string type {get; set;} = "";

	public ICollection<Trade> Trades {get;} = new List<Trade>();

}




