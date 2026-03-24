using MediatR;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;

namespace WeatherApp.Application.Weather.Queries;

public class GetHistoricalWeatherQueryHandler
    : IRequestHandler<GetHistoricalWeatherQuery, WeatherForecast>
{
    private readonly IWeatherService _weatherService;

    public GetHistoricalWeatherQueryHandler(IWeatherService weatherService)
        => _weatherService = weatherService;

    public Task<WeatherForecast> Handle(
        GetHistoricalWeatherQuery request,
        CancellationToken cancellationToken)
        => _weatherService.GetHistoricalWeatherAsync(
            request.Location, request.StartDate, request.EndDate);
}
