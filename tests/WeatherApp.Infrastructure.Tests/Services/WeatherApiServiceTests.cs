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

public class WeatherApiServiceTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public WeatherApiServiceTests()
    {
        // Common Arrange setup
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["WeatherApi:ApiKey"]).Returns("test-api-key");
        
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    }

    private WeatherApiService CreateService()
    {
        // BaseAddress is required to resolve the relative URI 'current.json...' used in the service
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.weatherapi.com/v1/")
        };
        return new WeatherApiService(httpClient, _configMock.Object);
    }

    [Fact]
    public void Constructor_MissingApiKey_ThrowsArgumentNullException()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["WeatherApi:ApiKey"]).Returns((string)null!);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new WeatherApiService(new HttpClient(), configMock.Object));
        Assert.Contains("WeatherApi:ApiKey is missing", exception.Message);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_ValidResponse_ReturnsWeather()
    {
        // Arrange
        var location = "London";
        var weatherApiResponse = new 
        {
            location = new { name = "London" },
            current = new { temp_c = 20.5, condition = new { text = "Partly cloudy" } }
        };
        var jsonResponse = JsonSerializer.Serialize(weatherApiResponse);

        var expectedUri = new Uri($"https://api.weatherapi.com/v1/current.json?key=test-api-key&q={location}");

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
                Content = new StringContent(jsonResponse)
            });

        var service = CreateService();

        // Act
        var result = await service.GetCurrentWeatherAsync(location);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("London", result.Location);
        Assert.Equal(20.5, result.TemperatureC);
        Assert.Equal("Partly cloudy", result.Description);
        
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == expectedUri),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_LocationWithSpaces_CorrectlyEncodesUrl()
    {
        // Arrange
        var location = "New York";
        var expectedUri = new Uri($"https://api.weatherapi.com/v1/current.json?key=test-api-key&q=New%20York"); // Edge case: ensuring 'Uri.EscapeDataString' is doing its job

        var weatherApiResponse = new 
        {
            location = new { name = "New York" },
            current = new { temp_c = 15.0, condition = new { text = "Clear" } }
        };

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
                Content = new StringContent(JsonSerializer.Serialize(weatherApiResponse))
            });

        var service = CreateService();

        // Act
        var result = await service.GetCurrentWeatherAsync(location);

        // Assert
        Assert.Equal("New York", result.Location);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_ApiReturnsErrorStatus_ThrowsHttpRequestException()
    {
        // Arrange
        var location = "InvalidLocation";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound // 404 will cause EnsureSuccessStatusCode to throw
            });

        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetCurrentWeatherAsync(location));
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_InvalidJsonResponseModel_ThrowsException()
    {
        // Arrange
        var location = "London";
        var jsonResponse = "{}"; // Missing location and current properties

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
                Content = new StringContent(jsonResponse)
            });

        var service = CreateService();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => service.GetCurrentWeatherAsync(location));
        Assert.Equal("Invalid response from weather API", exception.Message);
    }
}
