using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Infrastructure.Models;

namespace WeatherApp.Infrastructure.Services;

public class WeatherApiService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public WeatherApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["WeatherApi:ApiKey"] ?? throw new ArgumentNullException("WeatherApi:ApiKey is missing");
    }

    public async Task<Weather> GetCurrentWeatherAsync(string location)
    {
        var response = await _httpClient.GetAsync($"current.json?key={_apiKey}&q={Uri.EscapeDataString(location)}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(content);

        if (weatherData?.Location == null || weatherData.Current == null)
        {
            throw new Exception("Invalid response from weather API");
        }

        return new Weather
        {
            Location = weatherData.Location.Name,
            TemperatureC = weatherData.Current.TempC,
            Description = weatherData.Current.Condition?.Text ?? "Unknown",
            LastUpdated = DateTime.UtcNow
        };
    }

    public async Task<WeatherForecast> GetWeatherForecastAsync(string location)
    {
        var response = await _httpClient.GetAsync($"forecast.json?key={_apiKey}&q={Uri.EscapeDataString(location)}&days=7");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var forecastData = JsonSerializer.Deserialize<WeatherForecastApiResponse>(content);

        if (forecastData?.Location == null || forecastData.Forecast == null)
        {
            throw new Exception("Invalid response from weather forecast API");
        }

        var days = forecastData.Forecast.ForecastDay.Select(d => new WeatherForecastDay
        {
            Date = DateOnly.Parse(d.Date),
            MaxTempC = d.Day?.MaxTempC ?? 0,
            MinTempC = d.Day?.MinTempC ?? 0,
            Description = d.Day?.Condition?.Text ?? "Unknown"
        }).ToList();

        return new WeatherForecast
        {
            Location = forecastData.Location.Name,
            Days = days
        };
    }

    public async Task<WeatherForecast> GetHistoricalWeatherAsync(
        string location, DateOnly startDate, DateOnly endDate)
    {
        var url = $"history.json?key={_apiKey}" +
                  $"&q={Uri.EscapeDataString(location)}" +
                  $"&dt={startDate:yyyy-MM-dd}" +
                  $"&end_dt={endDate:yyyy-MM-dd}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var historyData = JsonSerializer.Deserialize<WeatherForecastApiResponse>(content);

        if (historyData?.Location == null || historyData.Forecast == null)
            throw new InvalidOperationException("Invalid response from historical weather API");

        var days = historyData.Forecast.ForecastDay.Select(d => new WeatherForecastDay
        {
            Date = DateOnly.Parse(d.Date),
            MaxTempC = d.Day?.MaxTempC ?? 0,
            MinTempC = d.Day?.MinTempC ?? 0,
            Description = d.Day?.Condition?.Text ?? "Unknown"
        }).ToList();

        return new WeatherForecast
        {
            Location = historyData.Location.Name,
            Days = days
        };
    }
}
