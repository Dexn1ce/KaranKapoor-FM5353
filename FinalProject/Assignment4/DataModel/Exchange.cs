using System;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;



[Table("exchange")]
public class Exchange
{
	[Key]
	[Column("exchangeid")]
	public int id {get; set;}
	
	[Column("name")]
	public string Name{get; set;} ="";
	[Column("country")]
	public string Country {get; set;} ="";

	[Column("createdat")]
	public DateTime createDate {get; set;}
	[Column("updatedat")]
	public DateTime updateDate {get; set;}
	
	// Markets being traded on the exchange 
	[NotMapped]
	public List<Market> Markets {get;} = new List<Market>(); 	


	public Exchange() {}

}
