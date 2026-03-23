using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using WeatherApp.Application.Weather.Queries;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Tests.Weather.Queries;

public class GetWeatherForecastQueryHandlerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly GetWeatherForecastQueryHandler _handler;

    public GetWeatherForecastQueryHandlerTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _handler = new GetWeatherForecastQueryHandler(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidLocation_ReturnsForecastResult()
    {
        // Arrange
        var location = "London";
        var query = new GetWeatherForecastQuery(location);
        var expectedForecast = new WeatherForecast
        {
            Location = location,
            Days =
            [
                new WeatherForecastDay { Date = new DateOnly(2025, 1, 1), MaxTempC = 10, MinTempC = 5, Description = "Sunny" },
                new WeatherForecastDay { Date = new DateOnly(2025, 1, 2), MaxTempC = 12, MinTempC = 6, Description = "Cloudy" }
            ]
        };

        _weatherServiceMock
            .Setup(s => s.GetWeatherForecastAsync(location))
            .ReturnsAsync(expectedForecast);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(location, result.Location);
        Assert.Equal(2, result.Days.Count);
        Assert.Equal("Sunny", result.Days[0].Description);
        _weatherServiceMock.Verify(s => s.GetWeatherForecastAsync(location), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        var location = "UnknownCity";
        var query = new GetWeatherForecastQuery(location);

        _weatherServiceMock
            .Setup(s => s.GetWeatherForecastAsync(location))
            .ThrowsAsync(new Exception("Forecast service failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Equal("Forecast service failed", exception.Message);
        _weatherServiceMock.Verify(s => s.GetWeatherForecastAsync(location), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceReturnsEmptyDays_ReturnsEmptyForecast()
    {
        // Arrange
        var location = "EmptyCity";
        var query = new GetWeatherForecastQuery(location);
        var emptyForecast = new WeatherForecast { Location = location, Days = [] };

        _weatherServiceMock
            .Setup(s => s.GetWeatherForecastAsync(location))
            .ReturnsAsync(emptyForecast);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(location, result.Location);
        Assert.Empty(result.Days);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Handle_InvalidLocation_PropagatesServiceBehavior(string? location)
    {
        // Arrange
        var query = new GetWeatherForecastQuery(location!);

        _weatherServiceMock
            .Setup(s => s.GetWeatherForecastAsync(location!))
            .ReturnsAsync((WeatherForecast)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _weatherServiceMock.Verify(s => s.GetWeatherForecastAsync(location!), Times.Once);
    }
}
