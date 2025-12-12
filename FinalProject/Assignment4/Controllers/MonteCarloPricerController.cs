using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;
using Assignment4 ;
using Assignment4.Services;
using Assignment4.Data;


using Npgsql;
using System.Linq;

namespace Assignment4.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MonteCarloPricerController : ControllerBase
{

    private readonly ILogger<MonteCarloPricerController> _logger;

    private readonly MCPricer _pricer;

    private int GetLatestRateCurveId(optionContext db, DateTime expiryDate)
    {
	    var opts = db.Options.ToList();
	    return db.ratecurve
		     .AsEnumerable()
		     .Where(c => DateTime.SpecifyKind(c.CurveDate,DateTimeKind.Utc) <= expiryDate)
		     .OrderByDescending(c => c.CurveDate)
		     .Select(c => c.RateCurveId)
		     .FirstOrDefault();
    }


	private double Timeleft(DateTime expdate)
	{
		TimeSpan diff = expdate.Date - DateTime.Now;
		double totDays = diff.TotalDays;
		return totDays/ 365;
	}

	private double Interpolate(List<ratePoint> points, double tenor)
	{
		var sorted  = points.OrderBy( p => p.Tenor).ToList();

		if(tenor <= sorted.First().Tenor)
			return sorted.First().Rate;

		if(tenor >= sorted.Last().Tenor)
			return sorted.Last().Rate;

		ratePoint? lower = null;

		ratePoint? upper = null;

		for (int i = 0; i < sorted.Count - 1; i++)
		    {
			if (sorted[i].Tenor <= tenor && tenor <= sorted[i + 1].Tenor)
			{
			    lower = sorted[i];
			    upper = sorted[i + 1];
			    break;
			}
		    }


		double T1 = lower!.Tenor;
		double R1 = lower!.Rate;

		double T2 = upper!.Tenor;
		double R2 = upper!.Rate;

		double weight = (tenor - T1)/(T2 - T1);

		return R1 + weight*(R2-R1);
	}


    public MonteCarloPricerController(MCPricer pricer, ILogger<MonteCarloPricerController> logger)
    {
        _logger = logger;
	_pricer = pricer;
    }

    [HttpGet("exchange")]
    public IEnumerable<Exchange> GetExchange()
    {
	    optionContext db = new optionContext();
	    return db.exchanges.ToList();
    }

    
    [HttpGet("exchange/{id}")]
    public IActionResult GetExchangeId(int id)
    {
	    using var db = new optionContext();

	    var result = db.exchanges
		           .FirstOrDefault(x => x.id == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
    }

    [HttpPost("exchange")]
    public async Task<IActionResult> PostExchange([FromBody] ExchangeDTO dto)
    {
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);

	    using var db =new  optionContext();


	    DateTime utc = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

	    var entity = new Exchange
	    {
		    Name =  dto.Name,
		    Country = dto.Country,
		    createDate = utc,
		    updateDate = utc,
	    };

	    db.exchanges.Add(entity);
	    await db.SaveChangesAsync();


	    return CreatedAtAction(
		nameof(GetExchangeId),
		new {id = entity.id},
		entity
		);
    } 


	[HttpPut("exchange/{id}")]
	public async Task<IActionResult> PutExchange(int id, [FromBody] ExchangeDTO dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = await db.exchanges.FindAsync(id);
	    if (entity == null)
		return NotFound($"Exchange with id {id} not found.");

	    entity.Name = dto.Name;
	    entity.Country = dto.Country;
	    entity.updateDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

	    db.exchanges.Update(entity);
	    await db.SaveChangesAsync();

	    return NoContent(); 
	}


	[HttpDelete("exchange/{id}")]
	public async Task<IActionResult> DeleteExchange(int id)
	{
	    using var db = new optionContext();

	    // Look up the entity by primary key
	    var entity = await db.exchanges.FindAsync(id);
	    if (entity == null)
		return NotFound($"Exchange with id {id} not found.");

	    // Remove entity
	    db.exchanges.Remove(entity);
	    await db.SaveChangesAsync();

	    return NoContent();  // Standard for DELETE
	}


	[HttpGet("market")]

    	public IEnumerable<Market> GetMarket()
	    {
		    optionContext db = new optionContext();
		    return db.markets.ToList();
	    }


    [HttpGet("market/{id}")]
    public IActionResult GetMarketId(int id)
    {
	    using var db = new optionContext();

	    var result = db.markets
		           .FirstOrDefault(x => x.id == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
    }


	[HttpPut("market/{id}")]
	public async Task<IActionResult> PutMarket(int id, [FromBody] MarketDTO dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = await db.markets.FindAsync(id);
	    if (entity == null)
		return NotFound($"Exchange with id {id} not found.");

	    entity.name = dto.Name;
	    entity.type = dto.Type;
	    entity.Exchangeid = dto.exchangeid;

	    db.markets.Update(entity);
	    await db.SaveChangesAsync();

	    return NoContent(); 
	}


	[HttpDelete("market/{id}")]
	public async Task<IActionResult> DeleteMarket(int id)
	{
	    using var db = new optionContext();

	    // Look up the entity by primary key
	    var entity = await db.markets.FindAsync(id);
	    if (entity == null)
		return NotFound($"Exchange with id {id} not found.");

	    // Remove entity
	    db.markets.Remove(entity);
	    await db.SaveChangesAsync();

	    return NoContent();  // Standard for DELETE
	}


	
    [HttpPost("market")]
    public async Task<IActionResult> PostMarket([FromBody] MarketDTO dto)
    {
	    if (!ModelState.IsValid)
		    return BadRequest(ModelState);

	    using var db =new  optionContext();



	    var entity = new Market
	    {
		    Exchangeid = dto.exchangeid,
		    name =  dto.Name,
		    type =  dto.Type
	    };

	    db.markets.Add(entity);
	    await db.SaveChangesAsync();


	    return CreatedAtAction(
		nameof(GetMarketId),
		new {id = entity.id},
		entity
		);
    } 








    [HttpGet("underlying")]
    public IEnumerable<underlying> GetUnderlying()
    {
	    optionContext db = new optionContext();
	    return db.underlying.ToList();
    }


    [HttpGet("underlying/{id}")]
    public IActionResult GetUnderlyingId(int id)
    {
	    using var db = new optionContext();

	    var result = db.underlying
		           .FirstOrDefault(x => x.underlyingid == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
    }

    [HttpPost("underlying")]
    public async Task<IActionResult> CreateUnderlying ([FromBody] UnderlyingCreateDto dto)
    {
	    if( !ModelState.IsValid)
		    return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = new underlying
	    {
		    symbol = dto.Symbol,
		    name = dto.Name,
		    assettype = dto.AssetType
	    };

	    db.underlying.Add(entity);
	    await db.SaveChangesAsync();

	    return CreatedAtAction(
		nameof(GetUnderlyingId),
		new {id = entity.underlyingid},
		entity
		);
    }

    	[HttpPut("underlying/{id}")]
	public IActionResult UpdateUnderlying(int id, [FromBody] UnderlyingCreateDto dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = db.underlying.Find(id);
	    if (entity == null)
		return NotFound($"Underlying with id {id} not found.");

	    // Update fields
	    entity.symbol = dto.Symbol;
	    entity.name = dto.Name;
	    entity.assettype = dto.AssetType;

	    db.underlying.Update(entity);
	    db.SaveChanges();

	    return NoContent();  
	}


	[HttpDelete("underlying/{id}")]
	public IActionResult DeleteUnderlying(int id)
	{
	    using var db = new optionContext();

	    var entity = db.underlying.Find(id);
	    if (entity == null)
		return NotFound($"Underlying with id {id} not found.");

	    db.underlying.Remove(entity);
	    db.SaveChanges();

	    return NoContent();  
	}

 
    [HttpGet("underlying/{id}/prices")]
    public IActionResult GetHistoricalPrices(int id)
    {
	    using var db = new optionContext();

	    var prices = db.underlying
		    	   .Include(b => b.prices)
			   .FirstOrDefault( u=> u.underlyingid == id);
	    if( prices == null) 
		    return NotFound();
	    return Ok(prices);
    }




        [HttpPost("underlying/{id}/prices")]
	public IActionResult AddHistoricalPrice(int id, [FromBody] HistoricalPriceUpdateDto dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var exists = db.price_series
               .Any(p => p.underlyingid == id && p.PriceTime.Date == dto.PriceTime.Date);

		if (exists)
    			return Conflict("A price already exists for this date.");


	    // Validate underlying exists
	    var underlying = db.underlying.Find(id);
	    if (underlying == null)
		return NotFound($"Underlying with id {id} not found.");

	    // Create new price row
	    var price = new historicalPrice
	    {
		underlyingid = id,            // FK
		PriceTime = dto.PriceTime,    // maps to your model
		LastPrice = dto.LastPrice
	    };

	    db.price_series.Add(price);
	    db.SaveChanges();

	    // Return 201 Created with a link back to GET historical prices
	    return CreatedAtAction(
		nameof(GetHistoricalPrices),
		new { id = id },
		price
	    );
	}

	[HttpPut("underlying/{id}/prices/{priceId}")]
	public IActionResult UpdateHistoricalPrice(int id, int priceId, [FromBody] HistoricalPriceUpdateDto dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    // Validate underlying exists
	    var underlying = db.underlying.Find(id);
	    if (underlying == null)
		return NotFound($"Underlying with id {id} not found.");

	    // Validate price exists AND belongs to this underlying
	    var entity = db.price_series
			   .FirstOrDefault(p => p.historicalpriceid == priceId && p.underlyingid == id);

	    if (entity == null)
		return NotFound($"Historical price {priceId} does not belong to underlying {id}.");

	    // Update fields
	    entity.PriceTime = dto.PriceTime;
	    entity.LastPrice = dto.LastPrice;

	    db.price_series.Update(entity);
	    db.SaveChanges();

	    return NoContent(); // REST standard
	}


	[HttpDelete("underlying/{id}/prices/{priceId}")]
	public IActionResult DeleteHistoricalPrice(int id, int priceId)
	{
	    using var db = new optionContext();

	    // Validate underlying exists
	    var underlying = db.underlying.Find(id);
	    if (underlying == null)
		return NotFound($"Underlying with id {id} not found.");

	    // Find price row belonging to this underlying
	    var entity = db.price_series
			   .FirstOrDefault(p => p.historicalpriceid == priceId && p.underlyingid == id);

	    if (entity == null)
		return NotFound($"Historical price {priceId} does not belong to underlying {id}.");

	    db.price_series.Remove(entity);
	    db.SaveChanges();

	    return NoContent();
	}












    [HttpGet("underlying/{id}/options")]
    public IActionResult GetOptions(int id)
    {
	    using var db = new optionContext();

	    var option = db.underlying
		    	   .Include(b => b.options)
			   .FirstOrDefault( u=> u.underlyingid == id);
	    if( option == null) 
		    return NotFound();
	    return Ok(option);
    }


    [HttpGet("underlying/{id}/asian_options")]
    public IActionResult GetAsianOptions(int id)
    {
	    using var db = new optionContext();

	    var option = db.underlying
		    	   .Include(b => b.asian_option)
			   .FirstOrDefault( u=> u.underlyingid == id);
	    if( option == null) 
		    return NotFound();
	    return Ok(option);
    }

    [HttpGet("options")]
    public IEnumerable<Option> GetEuropenOption()
    {
	    optionContext db = new optionContext();
	    return db.Options.ToList();
    }


    [HttpGet("underlying/options/{id}")]
    public IActionResult GetOptionsId(int id)
    {
	    using var db = new optionContext();

	    var result = db.Options
		           .FirstOrDefault(x => x.id == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
    }

    [HttpPost("options")]
    public IActionResult CreateOption([FromBody] OptionCreateDto opt)
    {

	    if( !ModelState.IsValid)
		    return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = new Option
	    {
		    K = opt.K,
		    sig = opt.sig,
		    b = opt.b,
		    expdate = opt.T,
		    otyp = opt.optiontype,
		    ratecurveid = opt.ratecurveid,
		    underlyingid = opt.underlyingid
	    };


	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.Options.Add(entity);
	    db.SaveChanges();




	    return CreatedAtAction(
		nameof(GetOptionsId),
		new {id = entity.id},
		entity
		);

    }
    	[HttpPut("options/{id}")]
	public IActionResult UpdateOption(int id, [FromBody] OptionCreateDto dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = db.Options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    // Update editable fields
	    entity.K = dto.K;
	    entity.sig = dto.sig;
	    entity.b = dto.b;
	    entity.expdate = dto.T;
	    entity.otyp = dto.optiontype;
	    entity.underlyingid = dto.underlyingid;

	    // Recompute latest rate curve for new expiration
	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.Options.Update(entity);
	    db.SaveChanges();

	    return NoContent();
	}

	[HttpDelete("options/{id}")]
	public IActionResult DeleteOption(int id)
	{
	    using var db = new optionContext();

	    var entity = db.Options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    db.Options.Remove(entity);
	    db.SaveChanges();

	    return NoContent();
	}




    [HttpPost("options/assign-lastest-ratecurve")]
    public async Task<IActionResult> AssignLatestRateCurve()
    {
	    using var db = new optionContext();


	    var opts = db.Options.ToList();

	    int latestId = 0;

	    foreach(var opt in opts)
	    {

	    	    latestId = GetLatestRateCurveId(db, opt.expdate);
		    opt.ratecurveid = latestId;
	    }

	    await db.SaveChangesAsync();

	    return Ok($"Assigned ratecurveid = {latestId} to {opts.Count} options.");
    }



    [HttpPost("asian_option/assign-lastest-ratecurve")]
    public async Task<IActionResult> AssignLatestRateCurveAO()
    {
	    using var db = new optionContext();


	    var opts = db.Asian_options.ToList();

	    int latestId = 0;

	    foreach(var opt in opts)
	    {

	    	    latestId = GetLatestRateCurveId(db, opt.expdate);
		    opt.ratecurveid = latestId;
	    }

	    await db.SaveChangesAsync();

	    return Ok($"Assigned ratecurveid = {latestId} to {opts.Count} options.");
    }


    [HttpPost("barrier_option/assign-lastest-ratecurve")]
    public async Task<IActionResult> AssignLatestRateCurveBO()
    {
	    using var db = new optionContext();


	    var opts = db.barrier_option.ToList();

	    int latestId = 0;

	    foreach(var opt in opts)
	    {

	    	    latestId = GetLatestRateCurveId(db, opt.expdate);
		    opt.ratecurveid = latestId;
	    }

	    await db.SaveChangesAsync();

	    return Ok($"Assigned ratecurveid = {latestId} to {opts.Count} options.");
    }

    [HttpPost("range_option/assign-lastest-ratecurve")]
    public async Task<IActionResult> AssignLatestRateCurveRO()
    {
	    using var db = new optionContext();


	    var opts = db.range_options.ToList();

	    int latestId = 0;

	    foreach(var opt in opts)
	    {

	    	    latestId = GetLatestRateCurveId(db, opt.expdate);
		    opt.ratecurveid = latestId;
	    }

	    await db.SaveChangesAsync();

	    return Ok($"Assigned ratecurveid = {latestId} to {opts.Count} options.");
    }


    [HttpPost("lookback_option/assign-lastest-ratecurve")]
    public async Task<IActionResult> AssignLatestRateCurveLO()
    {
	    using var db = new optionContext();


	    var opts = db.Lookback_options.ToList();

	    int latestId = 0;

	    foreach(var opt in opts)
	    {

	    	    latestId = GetLatestRateCurveId(db, opt.expdate);
		    opt.ratecurveid = latestId;
	    }

	    await db.SaveChangesAsync();

	    return Ok($"Assigned ratecurveid = {latestId} to {opts.Count} options.");
    }


    [HttpPost("digital_option/assign-lastest-ratecurve")]
    public async Task<IActionResult> AssignLatestRateCurveDO()
    {
	    using var db = new optionContext();


	    var opts = db.Digital_options.ToList();

	    int latestId = 0;

	    foreach(var opt in opts)
	    {

	    	    latestId = GetLatestRateCurveId(db, opt.expdate);
		    opt.ratecurveid = latestId;
	    }

	    await db.SaveChangesAsync();

	    return Ok($"Assigned ratecurveid = {latestId} to {opts.Count} options.");
    }


    [HttpGet("asian_option")]
    public IEnumerable<Asian_option> GetAsianOption()
    {
	    optionContext db = new optionContext();
	    return db.Asian_options.ToList();
    }

    [HttpGet("underlying/asian_option/{id}")]
    public IActionResult GetAsianOptionsId(int id)
    {
	    using var db = new optionContext();

	    var result = db.Asian_options
		           .FirstOrDefault(x => x.id == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
    }

    [HttpPost("asian_option")]
    public IActionResult CreateAsianOption([FromBody] OptionCreateDto opt)
    {

	    if( !ModelState.IsValid)
		    return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = new Asian_option
	    {
		    K = opt.K,
		    sig = opt.sig,
		    b = opt.b,
		    expdate = opt.T,
		    otyp = opt.optiontype,
		    ratecurveid = opt.ratecurveid,
		    underlyingid = opt.underlyingid
	    };


	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.Asian_options.Add(entity);
	    db.SaveChanges();




	    return CreatedAtAction(
		nameof(GetAsianOptionsId),
		new {id = entity.id},
		entity
		);

    }


    	[HttpPut("asian_options/{id}")]
	public IActionResult UpdateAsianOption(int id, [FromBody] OptionCreateDto dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = db.Asian_options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    // Update editable fields
	    entity.K = dto.K;
	    entity.sig = dto.sig;
	    entity.b = dto.b;
	    entity.expdate = dto.T;
	    entity.otyp = dto.optiontype;
	    entity.underlyingid = dto.underlyingid;

	    // Recompute latest rate curve for new expiration
	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.Asian_options.Update(entity);
	    db.SaveChanges();

	    return NoContent();
	}

	[HttpDelete("asian_options/{id}")]
	public IActionResult DeleteAsianOption(int id)
	{
	    using var db = new optionContext();

	    var entity = db.Asian_options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    db.Asian_options.Remove(entity);
	    db.SaveChanges();

	    return NoContent();
	}

    [HttpGet("digital_option")]
    public IEnumerable<Digital_option> GetDigitalOption()
    {
	    optionContext db = new optionContext();
	    return db.Digital_options.ToList();
    }


    [HttpGet("digital_option/{id}")]
    public IActionResult GetDigitalOptionsId(int id)
    {
	    using var db = new optionContext();

	    var result = db.Digital_options
		           .FirstOrDefault(x => x.id == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
    }


    [HttpPost("digital_option")]
    public IActionResult CreateDigitalOption([FromBody] OptionCreateDto opt)
    {

	    if( !ModelState.IsValid)
		    return BadRequest(ModelState);

	    using var db = new optionContext();

	    if(opt.payoutamount == null)
		    return BadRequest("Digital option must have a payoutamount");

	    var entity = new Digital_option
	    {
		    K = opt.K,
		    sig = opt.sig,
		    b = opt.b,
		    expdate = opt.T,
		    otyp = opt.optiontype,
		    payout_amount = opt.payoutamount ?? 0,
		    ratecurveid = opt.ratecurveid,
		    underlyingid = opt.underlyingid
	    };


	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.Digital_options.Add(entity);
	    db.SaveChanges();




	    return CreatedAtAction(
		nameof(GetDigitalOptionsId),
		new {id = entity.id},
		entity
		);

    }


    	[HttpPut("digital_option/{id}")]
	public IActionResult UpdateDigitalption(int id, [FromBody] OptionCreateDto dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = db.Digital_options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    // Update editable fields
	    entity.K = dto.K;
	    entity.sig = dto.sig;
	    entity.b = dto.b;
	    entity.expdate = dto.T;
	    entity.payout_amount = dto.payoutamount ?? 0;
	    entity.otyp = dto.optiontype;
	    entity.underlyingid = dto.underlyingid;

	    // Recompute latest rate curve for new expiration
	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.Digital_options.Update(entity);
	    db.SaveChanges();

	    return NoContent();
	}

	[HttpDelete("digital_option/{id}")]
	public IActionResult DeleteDigitalOption(int id)
	{
	    using var db = new optionContext();

	    var entity = db.Digital_options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    db.Digital_options.Remove(entity);
	    db.SaveChanges();

	    return NoContent();
	}


    [HttpGet("lookback_option")]
    public IEnumerable<Lookback_option> GetLookbackOption()
    {
	    optionContext db = new optionContext();
	    return db.Lookback_options.ToList();
    }


    [HttpGet("lookback_option/{id}")]
    public IActionResult GetLookbackOptionsId(int id)
    {
	    using var db = new optionContext();

	    var result = db.Lookback_options
		           .FirstOrDefault(x => x.id == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
    }


    [HttpPost("lookback_option")]
    public IActionResult CreateLookbackOption([FromBody] OptionCreateDto opt)
    {

	    if( !ModelState.IsValid)
		    return BadRequest(ModelState);

	    using var db = new optionContext();


	    var entity = new Lookback_option
	    {
		    K = opt.K,
		    sig = opt.sig,
		    b = opt.b,
		    expdate = opt.T,
		    otyp = opt.optiontype,
		    ratecurveid = opt.ratecurveid,
		    underlyingid = opt.underlyingid
	    };


	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.Lookback_options.Add(entity);
	    db.SaveChanges();




	    return CreatedAtAction(
		nameof(GetLookbackOptionsId),
		new {id = entity.id},
		entity
		);

    }


    	[HttpPut("lookback_option/{id}")]
	public IActionResult UpdateLookbackOption(int id, [FromBody] OptionCreateDto dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = db.Lookback_options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    // Update editable fields
	    entity.K = dto.K;
	    entity.sig = dto.sig;
	    entity.b = dto.b;
	    entity.expdate = dto.T;
	    entity.otyp = dto.optiontype;
	    entity.underlyingid = dto.underlyingid;

	    // Recompute latest rate curve for new expiration
	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.Lookback_options.Update(entity);
	    db.SaveChanges();

	    return NoContent();
	}

	[HttpDelete("lookback_option/{id}")]
	public IActionResult DeleteLookbackOption(int id)
	{
	    using var db = new optionContext();

	    var entity = db.Lookback_options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    db.Lookback_options.Remove(entity);
	    db.SaveChanges();

	    return NoContent();
	}


    [HttpGet("barrier_option")]
    public IEnumerable<Barrier_option> GetBarrierOption()
    {
	    optionContext db = new optionContext();
	    return db.barrier_option.ToList();
    }


    [HttpGet("barrier_option/{id}")]
    public IActionResult GetBarrierOptionsId(int id)
    {
	    using var db = new optionContext();

	    var result = db.barrier_option
		           .FirstOrDefault(x => x.id == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
    }


    [HttpPost("barrier_option")]
    public IActionResult CreateBarrierOption([FromBody] OptionCreateDto opt)
    {

	    if( !ModelState.IsValid)
		    return BadRequest(ModelState);

	    using var db = new optionContext();

	    if(opt.barrierlevel == null || opt.barrierlevel == null)
		    return BadRequest("Barrier option must have a barrierlevel or barriertype");

	    var entity = new Barrier_option
	    {
		    K = opt.K,
		    sig = opt.sig,
		    b = opt.b,
		    expdate = opt.T,
		    otyp = opt.optiontype,
		    BarrierType = opt.barriertype ?? "",
		    Barrier = opt.barrierlevel ?? 0,
		    ratecurveid = opt.ratecurveid,
		    underlyingid = opt.underlyingid
	    };


	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.barrier_option.Add(entity);
	    db.SaveChanges();




	    return CreatedAtAction(
		nameof(GetBarrierOptionsId),
		new {id = entity.id},
		entity
		);

    }


    	[HttpPut("barrier_option/{id}")]
	public IActionResult UpdateBarrierOption(int id, [FromBody] OptionCreateDto dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = db.barrier_option.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    // Update editable fields
	    entity.K = dto.K;
	    entity.sig = dto.sig;
	    entity.b = dto.b;
	    entity.expdate = dto.T;
	    entity.Barrier = dto.barrierlevel ?? 0;
     	    entity.BarrierType = dto.barriertype ?? "";
	    entity.otyp = dto.optiontype;
	    entity.underlyingid = dto.underlyingid;

	    // Recompute latest rate curve for new expiration
	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.barrier_option.Update(entity);
	    db.SaveChanges();

	    return NoContent();
	}

	[HttpDelete("barrier_option/{id}")]
	public IActionResult DeleteBarrierOption(int id)
	{
	    using var db = new optionContext();

	    var entity = db.barrier_option.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    db.barrier_option.Remove(entity);
	    db.SaveChanges();

	    return NoContent();
	}


    [HttpGet("range_option")]
    public IEnumerable<Range_option> GetRangeOption()
    {
	    optionContext db = new optionContext();
	    return db.range_options.ToList();
    }


    [HttpGet("range_option/{id}")]
    public IActionResult GetRangeOptionsId(int id)
    {
	    using var db = new optionContext();

	    var result = db.range_options
		           .FirstOrDefault(x => x.id == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
    }


    [HttpPost("range_option")]
    public IActionResult CreateRangeOption([FromBody] OptionCreateDto opt)
    {

	    if( !ModelState.IsValid)
		    return BadRequest(ModelState);

	    using var db = new optionContext();


	    var entity = new Range_option
	    {
		    sig = opt.sig,
		    b = opt.b,
		    expdate = opt.T,
		    ratecurveid = opt.ratecurveid,
		    underlyingid = opt.underlyingid
	    };


	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.range_options.Add(entity);
	    db.SaveChanges();




	    return CreatedAtAction(
		nameof(GetRangeOptionsId),
		new {id = entity.id},
		entity
		);

    }


    	[HttpPut("range_option/{id}")]
	public IActionResult UpdateRangeOption(int id, [FromBody] OptionCreateDto dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = db.range_options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    // Update editable fields
	    entity.sig = dto.sig;
	    entity.b = dto.b;
	    entity.expdate = dto.T;
	    entity.underlyingid = dto.underlyingid;

	    // Recompute latest rate curve for new expiration
	    entity.ratecurveid = GetLatestRateCurveId(db, entity.expdate);

	    db.range_options.Update(entity);
	    db.SaveChanges();

	    return NoContent();
	}

	[HttpDelete("range_option/{id}")]
	public IActionResult DeleteRangeOption(int id)
	{
	    using var db = new optionContext();

	    var entity = db.range_options.Find(id);
	    if (entity == null)
		return NotFound($"Option with id {id} not found.");

	    db.range_options.Remove(entity);
	    db.SaveChanges();

	    return NoContent();
	}

	[HttpGet("trade")]
	public IActionResult GetAllTrades()
	{
	    using var db = new optionContext();

	    var trades = db.trades
			   .Include(t => t.Underlying)
			   .Include(t => t.Market)
			   .ToList();

	    return Ok(trades);
	}



	[HttpGet("trade/{id}")]
	public IActionResult GetTradeId(int id)
	{
	    using var db = new optionContext();

	    var entity = db.trades
			   .Include(t => t.Underlying)
			   .Include(t => t.Market)
			   .FirstOrDefault(t => t.TradeId == id);

	    if (entity == null)
		return NotFound();

	    return Ok(entity);
	}




	[HttpPost("trade")]
	public IActionResult CreateTrade([FromBody] TradeCreateDTO dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    // Validate underlying
	    if (!db.underlying.Any(u => u.underlyingid == dto.underlyingid))
		return NotFound("Underlying does not exist.");

	    // Validate market
	    if (!db.markets.Any(m => m.id == dto.marketid))
		return NotFound("Market does not exist.");


	    var entity = new Trade
	    {
		UnderlyingId = dto.underlyingid,
		MarketId = dto.marketid,
		Direction = dto.direction,
		Quantity = dto.quantity,
		TradePrice = dto.tradeprice,
		TradeTime = dto.tradetime
	    };

	    db.trades.Add(entity);
	    db.SaveChanges();

	    return CreatedAtAction(nameof(GetTradeId), new { id = entity.TradeId }, entity);
	}

	[HttpPut("trade/{id}")]
	public IActionResult UpdateTrade(int id, [FromBody] TradeCreateDTO dto)
	{
	    if (!ModelState.IsValid)
		return BadRequest(ModelState);

	    using var db = new optionContext();

	    var entity = db.trades.Find(id);
	    if (entity == null)
		return NotFound();

	    entity.Direction = dto.direction;
	    entity.Quantity = dto.quantity;
	    entity.TradePrice = dto.tradeprice;
	    entity.TradeTime = dto.tradetime;

	    db.trades.Update(entity);
	    db.SaveChanges();

	    return NoContent();
	}

	[HttpDelete("trade/{id}")]
	public IActionResult DeleteTrade(int id)
	{
	    using var db = new optionContext();

	    var entity = db.trades.Find(id);
	    if (entity == null)
		return NotFound();

	    db.trades.Remove(entity);
	    db.SaveChanges();

	    return NoContent();
	}







    [HttpGet("ratecurve/{id}")]
    public IActionResult GetRateCurve(int id)
    {

	    using var db = new optionContext();

	    var result = db.ratecurve
		           .FirstOrDefault(x => x.RateCurveId == id);
	    if(result == null)
		    return NotFound();

	    return Ok(result);
	    
    }



    [HttpGet("ratecurve/{date}/ratepoints")]
    public IActionResult GetRatePoints(DateTime date)
    {
	    DateTime utc = DateTime.SpecifyKind(date, DateTimeKind.Utc);

	    using var db = new optionContext();

	    var points = db.ratecurve
		           .Include( r => r.RatePoints)
			   .Where( u=> u.CurveDate.Date == utc.Date)
			   .FirstOrDefault();
	    if( points == null) 
		    return NotFound();
	    return Ok(points);
    }

    [HttpGet("price_options/{id}")]
    public ActionResult<(double mean, double stderr)> GetMCdbPricer(int id, [FromQuery] MCsim mcparam)
    {
	    TobePricedOptionClass opt = new TobePricedOptionClass();

	    optionContext db = new optionContext();

	    var options = db.Options
		            .Include( a => a.underlying!)
			        .ThenInclude(b => b.prices)
			    .Include( b => b.Ratecurve!)
			        .ThenInclude(rc => rc.RatePoints!)
			    .Where(o => o.id == id )
			    .ToList();

	    if(options == null)
	    {
		    return NotFound("Option not found");
	    }
	    

	    


	    foreach(var Eopt in options)
	    {
		    opt.OptionType = "european";
		    opt.K = Eopt.K;
		    opt.sig = Eopt.sig;
		    opt.b = Eopt.b;
		    opt.T = Eopt.expdate;
		    opt.optiontyp = Eopt.otyp;
		    opt.N = mcparam.N;
		    opt.M = mcparam.M;
		    opt.useAnt = mcparam.useAnt;
		    opt.useCont = mcparam.useCont;
		    opt.isParallel = mcparam.isParallel;

		    var opTenor = Timeleft(opt.T);

		    if(Eopt!.Ratecurve == null)
			    return BadRequest("Please assign ratecurve to the option first");

		    opt.r = Interpolate(Eopt!.Ratecurve.RatePoints!.ToList(), opTenor);
		    
		    var priceAtTime = Eopt!.underlying!.prices
			    .FirstOrDefault( p=> p.PriceTime.Date == mcparam.simDate);

		    if(priceAtTime == null)
			    return BadRequest("Choose date between 2025-12-05 and 2025-11-11");
		    opt.S = priceAtTime.LastPrice;
	    }


	    var result = _pricer.PriceOption(opt);


	    if (result.mean == 0 && result.stderr == 0)
		    return StatusCode(500, "Pricing engine returned no output.");

	    if(opt.N <=0 || opt.M <= 0)
		    return BadRequest("N and M must be positive integers.");

	    if(string.IsNullOrEmpty(opt.optiontyp))
		    return BadRequest("OptionType is required.");


	    var greeks = _pricer.ComputeGreeks(opt);

	    return Ok(new
	    {
		result.mean,
		result.stderr,
		greeks.Delta,
		greeks.Vega,
		greeks.Rho,
		greeks.Theta
	    });




    }


    [HttpGet("price_asian_options/{id}")]
    public ActionResult<(double mean, double stderr)> GetMCdbPricerAO(int id, [FromQuery] MCsim mcparam)
    {
	    TobePricedOptionClass opt = new TobePricedOptionClass();

	    optionContext db = new optionContext();

	    var options = db.Asian_options
		            .Include( a => a.Underlying!)
			        .ThenInclude(b => b.prices)
			    .Include( b => b.ratecurve!)
			        .ThenInclude(rc => rc.RatePoints!)
			    .Where(o => o.id == id )
			    .ToList();

	    if(options == null)
	    {
		    return NotFound("Option not found");
	    }
	    

	    


	    foreach(var Aopt in options)
	    {
		    opt.OptionType = "asian";
		    opt.K = Aopt.K;
		    opt.sig = Aopt.sig;
		    opt.b = Aopt.b;
		    opt.T = Aopt.expdate;
		    opt.optiontyp = Aopt.otyp;
		    opt.N = mcparam.N;
		    opt.M = mcparam.M;
		    opt.useAnt = mcparam.useAnt;
		    opt.useCont = mcparam.useCont;
		    opt.isParallel = mcparam.isParallel;

		    var opTenor = Timeleft(opt.T);

		    if(Aopt!.ratecurve == null)
			    return BadRequest("Please assign ratecurve to the option first");


		    opt.r = Interpolate(Aopt!.ratecurve.RatePoints!.ToList(), opTenor);
		    
		    var priceAtTime = Aopt!.Underlying!.prices
			    .FirstOrDefault( p=> p.PriceTime.Date == mcparam.simDate);

		    if(priceAtTime == null)
			    return BadRequest("Choose date between 2025-12-05 and 2025-11-11");
		    opt.S = priceAtTime.LastPrice;
	    }


	    var result = _pricer.PriceOption(opt);


	    if (result.mean == 0 && result.stderr == 0)
		    return StatusCode(500, "Pricing engine returned no output.");

	    if(opt.N <=0 || opt.M <= 0)
		    return BadRequest("N and M must be positive integers.");

	    if(string.IsNullOrEmpty(opt.optiontyp))
		    return BadRequest("OptionType is required.");


	    var greeks = _pricer.ComputeGreeks(opt);

	    return Ok(new
	    {
		result.mean,
		result.stderr,
		greeks.Delta,
		greeks.Vega,
		greeks.Rho,
		greeks.Theta
	    });




    }

    [HttpGet("price_barrier_options/{id}")]
    public ActionResult<(double mean, double stderr)> GetMCdbPricerBO(int id, [FromQuery] MCsim mcparam)
    {
	    TobePricedOptionClass opt = new TobePricedOptionClass();

	    optionContext db = new optionContext();

	    var options = db.barrier_option
		            .Include( a => a.underlying!)
			        .ThenInclude(b => b.prices)
			    .Include( b => b.Ratecurve!)
			        .ThenInclude(rc => rc.RatePoints!)
			    .Where(o => o.id == id )
			    .ToList();

	    if(options == null)
	    {
		    return NotFound("Option not found");
	    }
	    

	    


	    foreach(var Aopt in options)
	    {
		    opt.OptionType = "barrier";
		    opt.K = Aopt.K;
		    opt.sig = Aopt.sig;
		    opt.b = Aopt.b;
		    opt.T = Aopt.expdate;
		    opt.optiontyp = Aopt.otyp;
		    opt.barrierlevel = Aopt.Barrier;
		    opt.barriertype = Aopt.BarrierType;
		    opt.N = mcparam.N;
		    opt.M = mcparam.M;
		    opt.useAnt = mcparam.useAnt;
		    opt.useCont = mcparam.useCont;
		    opt.isParallel = mcparam.isParallel;

		    var opTenor = Timeleft(opt.T);

		    if(Aopt!.Ratecurve == null)
			    return BadRequest("Please assign ratecurve to the option first");


		    opt.r = Interpolate(Aopt!.Ratecurve.RatePoints!.ToList(), opTenor);
		    
		    var priceAtTime = Aopt!.underlying!.prices
			    .FirstOrDefault( p=> p.PriceTime.Date == mcparam.simDate);

		    if(priceAtTime == null)
			    return BadRequest("Choose date between 2025-12-05 and 2025-11-11");
		    opt.S = priceAtTime.LastPrice;
	    }


	    var result = _pricer.PriceOption(opt);


	    if (result.mean == 0 && result.stderr == 0)
		    return StatusCode(500, "Pricing engine returned no output.");

	    if(opt.N <=0 || opt.M <= 0)
		    return BadRequest("N and M must be positive integers.");

	    if(string.IsNullOrEmpty(opt.optiontyp))
		    return BadRequest("OptionType is required.");


	    var greeks = _pricer.ComputeGreeks(opt);

	    return Ok(new
	    {
		result.mean,
		result.stderr,
		greeks.Delta,
		greeks.Vega,
		greeks.Rho,
		greeks.Theta
	    });




    }


    [HttpGet("price_range_options/{id}")]
    public ActionResult<(double mean, double stderr)> GetMCdbPricerRO(int id, [FromQuery] MCsim mcparam)
    {
	    TobePricedOptionClass opt = new TobePricedOptionClass();

	    optionContext db = new optionContext();

	    var options = db.range_options
		            .Include( a => a.underlying!)
			        .ThenInclude(b => b.prices)
			    .Include( b => b.Ratecurve!)
			        .ThenInclude(rc => rc.RatePoints!)
			    .Where(o => o.id == id )
			    .ToList();

	    if(options == null)
	    {
		    return NotFound("Option not found");
	    }
	    

	    


	    foreach(var Aopt in options)
	    {
		    opt.OptionType = "range";
		    opt.sig = Aopt.sig;
		    opt.b = Aopt.b;
		    opt.T = Aopt.expdate;
		    opt.N = mcparam.N;
		    opt.M = mcparam.M;
		    opt.useAnt = mcparam.useAnt;
		    opt.useCont = mcparam.useCont;
		    opt.isParallel = mcparam.isParallel;

		    var opTenor = Timeleft(opt.T);

		    if(Aopt!.Ratecurve == null)
			    return BadRequest("Please assign ratecurve to the option first");


		    opt.r = Interpolate(Aopt!.Ratecurve.RatePoints!.ToList(), opTenor);
		    
		    var priceAtTime = Aopt!.underlying!.prices
			    .FirstOrDefault( p=> p.PriceTime.Date == mcparam.simDate);

		    if(priceAtTime == null)
			    return BadRequest("Choose date between 2025-12-05 and 2025-11-11");
		    opt.S = priceAtTime.LastPrice;
	    }

	    

	    var result = _pricer.PriceOption(opt);


	    if (result.mean == 0 && result.stderr == 0)
		    return StatusCode(500, "Pricing engine returned no output.");

	    if(opt.N <=0 || opt.M <= 0)
		    return BadRequest("N and M must be positive integers.");

	    var greeks = _pricer.ComputeGreeks(opt);

	    return Ok(new
	    {
		result.mean,
		result.stderr,
		greeks.Delta,
		greeks.Vega,
		greeks.Rho,
		greeks.Theta
	    });

    }

    
    [HttpGet("price_lookback_options/{id}")]
    public ActionResult<(double mean, double stderr)> GetMCdbPricerLO(int id, [FromQuery] MCsim mcparam)
    {
	    TobePricedOptionClass opt = new TobePricedOptionClass();

	    optionContext db = new optionContext();

	    var options = db.Lookback_options
		            .Include( a => a.underlying!)
			        .ThenInclude(b => b.prices)
			    .Include( b => b.Ratecurve!)
			        .ThenInclude(rc => rc.RatePoints!)
			    .Where(o => o.id == id )
			    .ToList();

	    if(options == null)
	    {
		    return NotFound("Option not found");
	    }
	    

	    


	    foreach(var Aopt in options)
	    {
		    opt.OptionType = "lookback";
		    opt.K = Aopt.K;
		    opt.sig = Aopt.sig;
		    opt.b = Aopt.b;
		    opt.T = Aopt.expdate;
		    opt.optiontyp = Aopt.otyp;
		    opt.N = mcparam.N;
		    opt.M = mcparam.M;
		    opt.useAnt = mcparam.useAnt;
		    opt.useCont = mcparam.useCont;
		    opt.isParallel = mcparam.isParallel;

		    var opTenor = Timeleft(opt.T);

		    if(Aopt!.Ratecurve == null)
			    return BadRequest("Please assign ratecurve to the option first");


		    opt.r = Interpolate(Aopt!.Ratecurve.RatePoints!.ToList(), opTenor);
		    
		    var priceAtTime = Aopt!.underlying!.prices
			    .FirstOrDefault( p=> p.PriceTime.Date == mcparam.simDate);

		    if(priceAtTime == null)
			    return BadRequest("Choose date between 2025-12-05 and 2025-11-11");
		    opt.S = priceAtTime.LastPrice;
	    }


	    var result = _pricer.PriceOption(opt);


	    if (result.mean == 0 && result.stderr == 0)
		    return StatusCode(500, "Pricing engine returned no output.");

	    if(opt.N <=0 || opt.M <= 0)
		    return BadRequest("N and M must be positive integers.");

	    if(string.IsNullOrEmpty(opt.optiontyp))
		    return BadRequest("OptionType is required.");


	    var greeks = _pricer.ComputeGreeks(opt);

	    return Ok(new
	    {
		result.mean,
		result.stderr,
		greeks.Delta,
		greeks.Vega,
		greeks.Rho,
		greeks.Theta
	    });




    }


    [HttpGet("price_digital_options/{id}")]
    public ActionResult<(double mean, double stderr)> GetMCdbPricerDO(int id, [FromQuery] MCsim mcparam)
    {
	    TobePricedOptionClass opt = new TobePricedOptionClass();

	    optionContext db = new optionContext();

	    var options = db.Digital_options
		            .Include( a => a.underlying!)
			        .ThenInclude(b => b.prices)
			    .Include( b => b.Ratecurve!)
			        .ThenInclude(rc => rc.RatePoints!)
			    .Where(o => o.id == id )
			    .ToList();

	    if(options == null)
	    {
		    return NotFound("Option not found");
	    }
	    

	    


	    foreach(var Aopt in options)
	    {
		    opt.OptionType = "digital";
		    opt.K = Aopt.K;
		    opt.sig = Aopt.sig;
		    opt.b = Aopt.b;
		    opt.T = Aopt.expdate;
		    opt.optiontyp = Aopt.otyp;
		    opt.payoutamount = Aopt.payout_amount;
		    opt.N = mcparam.N;
		    opt.M = mcparam.M;
		    opt.useAnt = mcparam.useAnt;
		    opt.useCont = mcparam.useCont;
		    opt.isParallel = mcparam.isParallel;

		    var opTenor = Timeleft(opt.T);

		    if(Aopt!.Ratecurve == null)
			    return BadRequest("Please assign ratecurve to the option first");


		    opt.r = Interpolate(Aopt!.Ratecurve.RatePoints!.ToList(), opTenor);
		    
		    var priceAtTime = Aopt!.underlying!.prices
			    .FirstOrDefault( p=> p.PriceTime.Date == mcparam.simDate);

		    if(priceAtTime == null)
			    return BadRequest("Choose date between 2025-12-05 and 2025-11-11");
		    opt.S = priceAtTime.LastPrice;
	    }



	    var result = _pricer.PriceOption(opt);


	    if (result.mean == 0 && result.stderr == 0)
		    return StatusCode(500, "Pricing engine returned no output.");

	    if(opt.N <=0 || opt.M <= 0)
		    return BadRequest("N and M must be positive integers.");

	    if(string.IsNullOrEmpty(opt.optiontyp))
		    return BadRequest("OptionType is required.");


	    var greeks = _pricer.ComputeGreeks(opt);

	    return Ok(new
	    {
		result.mean,
		result.stderr,
		greeks.Delta,
		greeks.Vega,
		greeks.Rho,
		greeks.Theta
	    });




    }



    [HttpGet("price")]
    public ActionResult<(double mean, double stderr)> Get([FromQuery] TobePricedOptionClass option)
    {
	    var result = _pricer.PriceOption(option);

	    if (result.mean == 0 && result.stderr == 0)
		    return StatusCode(500, "Pricing engine returned no output.");

	    if(option.N <=0 || option.M <= 0)
		    return BadRequest("N and M must be positive integers.");

	    if(string.IsNullOrEmpty(option.optiontyp))
		    return BadRequest("OptionType is required.");

	    return Ok(new {result.mean, result.stderr});
	    


    }

    [HttpGet("price_greeks")]
	public ActionResult PriceWithGreeks([FromQuery] TobePricedOptionClass option)
	{
	//    var entity = await _context.Options.FindAsync(id);
	/*    if (entity == null)
		return NotFound();*/

	 //   OptionBase opt = OptionFactory.Create(entity);

	    var price = _pricer.PriceOption(option).mean;
	    var greeks = _pricer.ComputeGreeks(option);

	    return Ok(new
	    {
		price,
		greeks.Delta,
		greeks.Vega,
		greeks.Rho,
		greeks.Theta
	    });
	}



}

