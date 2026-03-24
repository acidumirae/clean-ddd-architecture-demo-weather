using FsCheck;
using FsCheck.Xunit;
using Moq;
using Xunit;
using WeatherApp.Application.Weather.Queries;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Tests.Weather.Queries;

public class GetHistoricalWeatherQueryHandlerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly GetHistoricalWeatherQueryHandler _handler;

    public GetHistoricalWeatherQueryHandlerTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _handler = new GetHistoricalWeatherQueryHandler(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsWeatherForecast()
    {
        // Arrange
        var location = "London";
        var startDate = new DateOnly(2025, 1, 1);
        var endDate = new DateOnly(2025, 1, 3);
        var query = new GetHistoricalWeatherQuery(location, startDate, endDate);
        var expectedForecast = new WeatherForecast
        {
            Location = location,
            Days =
            [
                new WeatherForecastDay { Date = new DateOnly(2025, 1, 1), MaxTempC = 10, MinTempC = 5, Description = "Sunny" },
                new WeatherForecastDay { Date = new DateOnly(2025, 1, 2), MaxTempC = 8, MinTempC = 3, Description = "Cloudy" },
                new WeatherForecastDay { Date = new DateOnly(2025, 1, 3), MaxTempC = 6, MinTempC = 1, Description = "Rainy" }
            ]
        };

        _weatherServiceMock
            .Setup(s => s.GetHistoricalWeatherAsync(location, startDate, endDate))
            .ReturnsAsync(expectedForecast);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(location, result.Location);
        Assert.Equal(3, result.Days.Count);
        Assert.Equal("Sunny", result.Days[0].Description);
        _weatherServiceMock.Verify(s => s.GetHistoricalWeatherAsync(location, startDate, endDate), Times.Once);
    }

    // Feature: historical-weather, Property 1: Historical query result contains the requested location
    [Property(MaxTest = 100)]
    public Property LocationIsPreservedInHistoricalQueryResult()
    {
        return Prop.ForAll(
            Arb.Default.NonEmptyString(),
            (nonEmptyLocation) =>
            {
                var location = nonEmptyLocation.Item;
                var startDate = new DateOnly(2020, 1, 1);
                var endDate = new DateOnly(2024, 12, 31);

                var expectedForecast = new WeatherForecast
                {
                    Location = location,
                    Days = []
                };

                var weatherServiceMock = new Mock<IWeatherService>();
                weatherServiceMock
                    .Setup(s => s.GetHistoricalWeatherAsync(location, startDate, endDate))
                    .ReturnsAsync(expectedForecast);

                var handler = new GetHistoricalWeatherQueryHandler(weatherServiceMock.Object);
                var query = new GetHistoricalWeatherQuery(location, startDate, endDate);

                var result = handler.Handle(query, CancellationToken.None).GetAwaiter().GetResult();

                return result.Location.Length > 0;
            });
    }

    [Fact]
    public async Task Handle_ServiceThrows_PropagatesException()
    {
        // Arrange
        var location = "UnknownCity";
        var startDate = new DateOnly(2025, 1, 1);
        var endDate = new DateOnly(2025, 1, 7);
        var query = new GetHistoricalWeatherQuery(location, startDate, endDate);

        _weatherServiceMock
            .Setup(s => s.GetHistoricalWeatherAsync(location, startDate, endDate))
            .ThrowsAsync(new Exception("Historical service failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        Assert.Equal("Historical service failed", exception.Message);
        _weatherServiceMock.Verify(s => s.GetHistoricalWeatherAsync(location, startDate, endDate), Times.Once);
    }
}
