using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;
using WeatherApp.Infrastructure.Services;

namespace WeatherApp.Infrastructure.Tests.Services;

public class WeatherApiServiceForecastTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public WeatherApiServiceForecastTests()
    {
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["WeatherApi:ApiKey"]).Returns("test-api-key");
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    }

    private WeatherApiService CreateService()
    {
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.weatherapi.com/v1/")
        };
        return new WeatherApiService(httpClient, _configMock.Object);
    }

    private static string BuildForecastJson(string locationName, int dayCount = 7)
    {
        var days = Enumerable.Range(0, dayCount).Select(i => new
        {
            date = new DateOnly(2025, 1, 1).AddDays(i).ToString("yyyy-MM-dd"),
            day = new
            {
                maxtemp_c = 15.0 + i,
                mintemp_c = 5.0 + i,
                condition = new { text = i % 2 == 0 ? "Sunny" : "Cloudy" }
            }
        });

        return JsonSerializer.Serialize(new
        {
            location = new { name = locationName },
            forecast = new { forecastday = days }
        });
    }

    [Fact]
    public async Task GetWeatherForecastAsync_ValidResponse_ReturnsForecastWithDays()
    {
        // Arrange
        var location = "London";
        var json = BuildForecastJson("London", 7);
        var expectedUri = new Uri($"https://api.weatherapi.com/v1/forecast.json?key=test-api-key&q={location}&days=7");

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        var service = CreateService();

        // Act
        var result = await service.GetWeatherForecastAsync(location);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("London", result.Location);
        Assert.Equal(7, result.Days.Count);
        Assert.Equal(15.0, result.Days[0].MaxTempC);
        Assert.Equal(5.0, result.Days[0].MinTempC);
        Assert.Equal("Sunny", result.Days[0].Description);
    }

    [Fact]
    public async Task GetWeatherForecastAsync_LocationWithSpaces_CorrectlyEncodesUrl()
    {
        // Arrange
        var location = "New York";
        var json = BuildForecastJson("New York", 7);
        var expectedUri = new Uri("https://api.weatherapi.com/v1/forecast.json?key=test-api-key&q=New%20York&days=7");

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        var service = CreateService();

        // Act
        var result = await service.GetWeatherForecastAsync(location);

        // Assert
        Assert.Equal("New York", result.Location);
    }

    [Fact]
    public async Task GetWeatherForecastAsync_ApiReturnsErrorStatus_ThrowsHttpRequestException()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetWeatherForecastAsync("InvalidCity"));
    }

    [Fact]
    public async Task GetWeatherForecastAsync_InvalidJsonResponse_ThrowsException()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        var service = CreateService();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => service.GetWeatherForecastAsync("London"));
        Assert.Equal("Invalid response from weather forecast API", exception.Message);
    }

    [Fact]
    public async Task GetWeatherForecastAsync_DayWithNullCondition_UsesUnknownDescription()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            location = new { name = "Paris" },
            forecast = new
            {
                forecastday = new[]
                {
                    new { date = "2025-01-01", day = new { maxtemp_c = 10.0, mintemp_c = 3.0, condition = (object?)null } }
                }
            }
        });

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });

        var service = CreateService();

        // Act
        var result = await service.GetWeatherForecastAsync("Paris");

        // Assert
        Assert.Equal("Unknown", result.Days[0].Description);
    }
}
