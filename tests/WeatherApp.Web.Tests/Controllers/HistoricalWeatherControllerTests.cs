using FsCheck;
using FsCheck.Xunit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WeatherApp.Web.Controllers;
using WeatherApp.Application.Weather.Queries;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Web.Tests.Controllers;

public class HistoricalWeatherControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly HistoricalWeatherController _controller;

    public HistoricalWeatherControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new HistoricalWeatherController(_mediatorMock.Object);
    }

    [Fact]
    public void Index_Get_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.Model);
    }

    [Fact]
    public async Task Index_Post_ValidInput_ReturnsViewWithModel()
    {
        // Arrange
        var location = "London";
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 7);
        var expectedForecast = new WeatherForecast
        {
            Location = location,
            Days =
            [
                new WeatherForecastDay { Date = startDate, MaxTempC = 10, MinTempC = 2, Description = "Cloudy" }
            ]
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetHistoricalWeatherQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedForecast);

        // Act
        var result = await _controller.Index(location, startDate, endDate) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<WeatherForecast>(result.Model);
        Assert.Equal(location, model.Location);
        Assert.Single(model.Days);
    }

    [Fact]
    public async Task Index_Post_StartDateAfterEndDate_ReturnsViewWithModelError()
    {
        // Arrange
        var location = "Paris";
        var startDate = new DateOnly(2024, 1, 10);
        var endDate = new DateOnly(2024, 1, 1);

        // Act
        var result = await _controller.Index(location, startDate, endDate) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Model);
        Assert.False(result.ViewData.ModelState.IsValid);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetHistoricalWeatherQuery>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task Index_Post_EmptyLocation_ReturnsViewWithModelError()
    {
        // Arrange
        var location = "   ";
        var startDate = new DateOnly(2024, 1, 1);
        var endDate = new DateOnly(2024, 1, 7);

        // Act
        var result = await _controller.Index(location, startDate, endDate) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Model);
        Assert.False(result.ViewData.ModelState.IsValid);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetHistoricalWeatherQuery>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    // Feature: historical-weather, Property 3: View renders all forecast days
    [Property(MaxTest = 100)]
    public Property ViewModelContainsAllForecastDays()
    {
        return Prop.ForAll(
            Arb.From(Gen.Choose(0, 30)),
            (dayCount) =>
            {
                var location = "TestCity";
                var startDate = new DateOnly(2024, 1, 1);
                var endDate = startDate.AddDays(Math.Max(dayCount - 1, 0));

                var days = Enumerable.Range(0, dayCount)
                    .Select(i => new WeatherForecastDay
                    {
                        Date = startDate.AddDays(i),
                        MaxTempC = 10 + i,
                        MinTempC = 2 + i,
                        Description = "Sunny"
                    })
                    .ToList();

                var forecast = new WeatherForecast { Location = location, Days = days };

                var mediatorMock = new Mock<IMediator>();
                mediatorMock
                    .Setup(m => m.Send(It.IsAny<GetHistoricalWeatherQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(forecast);

                var controller = new HistoricalWeatherController(mediatorMock.Object);
                var result = controller.Index(location, startDate, endDate)
                    .GetAwaiter().GetResult() as ViewResult;

                var model = result?.Model as WeatherForecast;
                return model != null && model.Days.Count == dayCount;
            });
    }
}
