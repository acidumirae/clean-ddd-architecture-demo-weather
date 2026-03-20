using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using WeatherApp.Application.Weather.Queries;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Tests.Weather.Queries;

public class GetWeatherQueryHandlerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly GetWeatherQueryHandler _handler;

    public GetWeatherQueryHandlerTests()
    {
        // Arrange common setup
        _weatherServiceMock = new Mock<IWeatherService>();
        _handler = new GetWeatherQueryHandler(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidLocation_ReturnsWeatherResult()
    {
        // Arrange
        var location = "London";
        var query = new GetWeatherQuery(location);
        var expectedWeather = new WeatherApp.Domain.Entities.Weather
        {
            Location = location,
            TemperatureC = 20,
            Description = "Sunny",
            LastUpdated = DateTime.UtcNow
        };

        _weatherServiceMock
            .Setup(s => s.GetCurrentWeatherAsync(location))
            .ReturnsAsync(expectedWeather);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedWeather.Location, result.Location);
        Assert.Equal(expectedWeather.TemperatureC, result.TemperatureC);
        Assert.Equal(expectedWeather.Description, result.Description);
        
        _weatherServiceMock.Verify(s => s.GetCurrentWeatherAsync(location), Times.Once); // Explicit Dependency Principle validation
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_ThrowsExceptionPropagated()
    {
        // Arrange
        var location = "InvalidLocation";
        var query = new GetWeatherQuery(location);
        var expectedExceptionMessage = "Weather service failed";

        _weatherServiceMock
            .Setup(s => s.GetCurrentWeatherAsync(location))
            .ThrowsAsync(new Exception(expectedExceptionMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Equal(expectedExceptionMessage, exception.Message);
        
        _weatherServiceMock.Verify(s => s.GetCurrentWeatherAsync(location), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Handle_InvalidLocation_ReturnsNullWhenServiceReturnsNull(string location)
    {
        // Arrange
        var query = new GetWeatherQuery(location);
        
        // Simulating the IWeatherService returning null for an invalid/empty location
        _weatherServiceMock
            .Setup(s => s.GetCurrentWeatherAsync(location!))
            .ReturnsAsync((WeatherApp.Domain.Entities.Weather)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _weatherServiceMock.Verify(s => s.GetCurrentWeatherAsync(location!), Times.Once);
    }
}
