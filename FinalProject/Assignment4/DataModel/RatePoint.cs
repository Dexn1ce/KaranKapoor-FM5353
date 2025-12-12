
using System;

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;




[Table("ratepoint")]
public class ratePoint
{
	[Column("ratepointid")]
	public int RatePointId{get; set;}

	public int ratecurveid {get; set;}

	[JsonIgnore]
	[ForeignKey("ratecurveid")]
	public rateCurve? RateCurve {get; set;}
	[Column("tenor")]
	public double Tenor{get; set;}
	[Column("rate")]
	public double Rate{get; set;} 
}
