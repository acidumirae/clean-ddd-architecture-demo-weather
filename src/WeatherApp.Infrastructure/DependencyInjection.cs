using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Infrastructure.Services;

namespace WeatherApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["WeatherApi:BaseUrl"] ?? "https://api.weatherapi.com/v1/";

        services.AddHttpClient<IWeatherService, WeatherApiService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
