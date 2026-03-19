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
}
