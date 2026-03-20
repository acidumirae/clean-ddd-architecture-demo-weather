using MediatR;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Weather.Queries;

public record GetWeatherQuery(string Location) : IRequest<Domain.Entities.Weather>;
