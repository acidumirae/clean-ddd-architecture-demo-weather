using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WeatherApp.Web.Controllers;
using WeatherApp.Application.Weather.Queries;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Web.Tests.Controllers;

public class WeatherForecastControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly WeatherForecastController _controller;

    public WeatherForecastControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new WeatherForecastController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Index_ValidLocation_ReturnsViewResultWithWeatherForecastModel()
    {
        // Arrange
        var location = "Paris";
        var expectedForecast = new WeatherForecast
        {
            Location = location,
            Days =
            [
                new WeatherForecastDay { Date = new DateOnly(2025, 1, 1), MaxTempC = 10, MinTempC = 2, Description = "Sunny" }
            ]
        };

        _mediatorMock.Setup(m => m.Send(
            It.Is<GetWeatherForecastQuery>(q => q.Location == location),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedForecast);

        // Act
        var result = await _controller.Index(location) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<WeatherForecast>(result.Model);
        Assert.Equal(location, model.Location);
        Assert.Single(model.Days);
    }

    [Fact]
    public async Task Index_NoLocationProvided_UsesLondonDefaultAndReturnsViewResult()
    {
        // Arrange
        var expectedForecast = new WeatherForecast
        {
            Location = "London",
            Days = []
        };

        _mediatorMock.Setup(m => m.Send(
            It.Is<GetWeatherForecastQuery>(q => q.Location == "London"),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedForecast);

        // Act
        var result = await _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<WeatherForecast>(result.Model);
        Assert.Equal("London", model.Location);
    }

    [Fact]
    public async Task Index_MediatorThrowsException_ReturnsViewResultWithViewBagError()
    {
        // Arrange
        var location = "InvalidCity";
        var errorMessage = "Could not fetch forecast data";

        _mediatorMock.Setup(m => m.Send(
            It.Is<GetWeatherForecastQuery>(q => q.Location == location),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.Index(location) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Model);
        Assert.Equal(errorMessage, _controller.ViewBag.Error);
    }

    [Fact]
    public async Task Index_ValidLocation_SendsCorrectQueryToMediator()
    {
        // Arrange
        var location = "Tokyo";
        var expectedForecast = new WeatherForecast { Location = location, Days = [] };

        _mediatorMock.Setup(m => m.Send(
            It.IsAny<GetWeatherForecastQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedForecast);

        // Act
        await _controller.Index(location);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<GetWeatherForecastQuery>(q => q.Location == location),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
