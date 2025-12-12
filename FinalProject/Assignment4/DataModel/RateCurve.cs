
using System;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("ratecurve")]
public class rateCurve
{
	[Column("ratecurveid")]
	public int RateCurveId{get; set;} 
	[Column("curvename")]
	public string CurveName{get; set;} = "";
	[Column("currency")]
	public string Currency {get; set;} = "";

	[Column("curvedate")]
	public DateTime CurveDate {get; set;} 

	public ICollection<ratePoint> RatePoints {get; } = new List<ratePoint>();

	public double GetRateForMaturity(double T)
	{
		var nearest = RatePoints
			.OrderBy(p => Math.Abs(p.Tenor - T))
			.FirstOrDefault();

		if (nearest == null)
			throw new Exception("No rate points in curve!");
		return nearest.Rate;
	}
}
