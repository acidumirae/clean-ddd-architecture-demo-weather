using System;
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

public class WeatherControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly WeatherController _controller;

    public WeatherControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new WeatherController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Index_NoLocationProvided_UsesLondonDefaultAndReturnsViewResult()
    {
        // Arrange
        var expectedWeather = new WeatherApp.Domain.Entities.Weather { Location = "London", TemperatureC = 15 };
        
        _mediatorMock.Setup(m => m.Send(
            It.Is<GetWeatherQuery>(q => q.Location == "London"), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWeather);

        // Act
        // Calling Index without arguments to trigger the `location = "London"` default parameter logic
        var result = await _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedWeather, result.Model);
    }

    [Fact]
    public async Task Index_ValidLocation_ReturnsViewResultWithWeatherModel()
    {
        // Arrange
        var location = "Paris";
        var expectedWeather = new WeatherApp.Domain.Entities.Weather { Location = location, TemperatureC = 20 };

        _mediatorMock.Setup(m => m.Send(
            It.Is<GetWeatherQuery>(q => q.Location == location), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWeather);

        // Act
        var result = await _controller.Index(location) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<WeatherApp.Domain.Entities.Weather>(result.Model);
        Assert.Equal(location, model.Location);
    }

    [Fact]
    public async Task Index_MediatorThrowsException_ReturnsViewResultWithViewBagError()
    {
        // Arrange
        var location = "InvalidCity";
        var errorMessage = "Could not fetch weather data";
        
        _mediatorMock.Setup(m => m.Send(
            It.Is<GetWeatherQuery>(q => q.Location == location), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _controller.Index(location) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Model); // Ensure model is null as per catch logic
        
        // Ensure ViewBag.Error contains the exception message
        Assert.Equal(errorMessage, _controller.ViewBag.Error);
    }
}
