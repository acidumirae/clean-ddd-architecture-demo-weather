using WeatherApp.Domain.Entities;

namespace WeatherApp.Domain.Interfaces;

public interface IWeatherService
{
    Task<Weather> GetCurrentWeatherAsync(string location);
    Task<WeatherForecast> GetWeatherForecastAsync(string location);
}
