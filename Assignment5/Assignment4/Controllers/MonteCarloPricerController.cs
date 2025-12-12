using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Assignment4.Services;


using Npgsql;
using System.Linq;

namespace Assignment4.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MonteCarloPricerController : ControllerBase
{

    private readonly ILogger<MonteCarloPricerController> _logger;

    private readonly MCPricer _pricer;


    public MonteCarloPricerController(MCPricer pricer, ILogger<MonteCarloPricerController> logger)
    {
        _logger = logger;
	_pricer = pricer;
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

