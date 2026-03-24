using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;
using FsCheck;
using FsCheck.Xunit;
using WeatherApp.Infrastructure.Services;

namespace WeatherApp.Infrastructure.Tests.Services;

public class WeatherApiServiceHistoricalTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public WeatherApiServiceHistoricalTests()
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

    [Fact]
    public async Task GetHistoricalWeatherAsync_ValidResponse_MapsCorrectly()
    {
        // Arrange
        var location = "London";
        var startDate = new DateOnly(2025, 1, 1);
        var endDate = new DateOnly(2025, 1, 3);
        var expectedUri = new Uri(
            "https://api.weatherapi.com/v1/history.json?key=test-api-key&q=London&dt=2025-01-01&end_dt=2025-01-03");

        var json = JsonSerializer.Serialize(new
        {
            location = new { name = "London" },
            forecast = new
            {
                forecastday = new[]
                {
                    new { date = "2025-01-01", day = new { maxtemp_c = 10.0, mintemp_c = 3.0, condition = new { text = "Sunny" } } },
                    new { date = "2025-01-02", day = new { maxtemp_c = 11.0, mintemp_c = 4.0, condition = new { text = "Cloudy" } } },
                    new { date = "2025-01-03", day = new { maxtemp_c = 12.0, mintemp_c = 5.0, condition = new { text = "Rainy" } } }
                }
            }
        });

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
        var result = await service.GetHistoricalWeatherAsync(location, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("London", result.Location);
        Assert.Equal(3, result.Days.Count);
        Assert.Equal(10.0, result.Days[0].MaxTempC);
        Assert.Equal(3.0, result.Days[0].MinTempC);
        Assert.Equal("Sunny", result.Days[0].Description);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_NullLocation_ThrowsInvalidOperationException()
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
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GetHistoricalWeatherAsync("London", new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 3)));
        Assert.Equal("Invalid response from historical weather API", exception.Message);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_HttpError_ThrowsHttpRequestException()
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
        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetHistoricalWeatherAsync("London", new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 3)));
    }

    // Feature: historical-weather, Property 2: Historical query result days fall within the requested date range
    [Property(MaxTest = 100)]
    public Property DaysFallWithinRequestedDateRange()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(0, 365).Two().Select(pair =>
            {
                var baseDate = new DateOnly(2023, 1, 1);
                var d1 = baseDate.AddDays(Math.Min(pair.Item1, pair.Item2));
                var d2 = baseDate.AddDays(Math.Max(pair.Item1, pair.Item2));
                return (startDate: d1, endDate: d2);
            })),
            (range) =>
            {
                var (startDate, endDate) = range;

                // Build days strictly within the range
                var days = new List<DateOnly>();
                var current = startDate;
                while (current <= endDate && days.Count < 10)
                {
                    days.Add(current);
                    current = current.AddDays(1);
                }

                var forecastDays = days.Select(d => new
                {
                    date = d.ToString("yyyy-MM-dd"),
                    day = new { maxtemp_c = 15.0, mintemp_c = 5.0, condition = new { text = "Sunny" } }
                }).ToArray();

                var json = JsonSerializer.Serialize(new
                {
                    location = new { name = "TestCity" },
                    forecast = new { forecastday = forecastDays }
                });

                var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                handlerMock
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

                var configMock = new Mock<IConfiguration>();
                configMock.Setup(c => c["WeatherApi:ApiKey"]).Returns("test-key");

                var httpClient = new HttpClient(handlerMock.Object)
                {
                    BaseAddress = new Uri("https://api.weatherapi.com/v1/")
                };
                var service = new WeatherApiService(httpClient, configMock.Object);

                var result = service.GetHistoricalWeatherAsync("TestCity", startDate, endDate)
                    .GetAwaiter().GetResult();

                return result.Days.All(d => d.Date >= startDate && d.Date <= endDate);
            });
    }
}
