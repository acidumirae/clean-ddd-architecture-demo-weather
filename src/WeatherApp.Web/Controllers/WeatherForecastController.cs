using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Application.Weather.Queries;

namespace WeatherApp.Web.Controllers;

public class WeatherForecastController : Controller
{
    private readonly IMediator _mediator;

    public WeatherForecastController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string location = "London")
    {
        try
        {
            var query = new GetWeatherForecastQuery(location);
            var result = await _mediator.Send(query);
            return View(result);
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(null);
        }
    }
}
