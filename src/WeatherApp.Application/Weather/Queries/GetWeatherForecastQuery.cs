using MediatR;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Weather.Queries;

public record GetWeatherForecastQuery(string Location) : IRequest<WeatherForecast>;
