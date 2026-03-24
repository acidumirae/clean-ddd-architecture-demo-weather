using MediatR;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Weather.Queries;

public record GetHistoricalWeatherQuery(string Location, DateOnly StartDate, DateOnly EndDate)
    : IRequest<WeatherForecast>;
