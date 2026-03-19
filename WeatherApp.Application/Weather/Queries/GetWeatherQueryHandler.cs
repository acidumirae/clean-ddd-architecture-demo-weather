using MediatR;
using WeatherApp.Domain.Interfaces;

namespace WeatherApp.Application.Weather.Queries;

public class GetWeatherQueryHandler : IRequestHandler<GetWeatherQuery, Domain.Entities.Weather>
{
    private readonly IWeatherService _weatherService;

    public GetWeatherQueryHandler(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<Domain.Entities.Weather> Handle(GetWeatherQuery request, CancellationToken cancellationToken)
    {
        return await _weatherService.GetCurrentWeatherAsync(request.Location);
    }
}
