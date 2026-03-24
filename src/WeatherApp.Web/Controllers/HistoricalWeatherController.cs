using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Application.Weather.Queries;

namespace WeatherApp.Web.Controllers;

public class HistoricalWeatherController(IMediator mediator) : Controller
{
    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Index(string location, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            ModelState.AddModelError(nameof(location), "Location is required.");
            return View();
        }

        if (startDate > endDate)
        {
            ModelState.AddModelError(nameof(startDate), "Start date must be on or before end date.");
            return View();
        }

        try
        {
            var query = new GetHistoricalWeatherQuery(location, startDate, endDate);
            var result = await mediator.Send(query);
            return View(result);
        }
        catch (Exception ex)
        {
            ViewBag.Error = ex.Message;
            return View(null);
        }
    }
}
