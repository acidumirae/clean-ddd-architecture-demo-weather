using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using WeatherApp.Web.Controllers;
using WeatherApp.Web.Models;

namespace WeatherApp.Web.Tests.Controllers;

public class HomeControllerTests
{
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _controller = new HomeController();
        
        // Setup a mock HttpContext for testing TraceIdentifier in the Error action
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ReturnsViewResultWithViewModel()
    {
        // Arrange (HttpContext arranged in constructor)

        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<ErrorViewModel>(result.Model);
        Assert.Equal("test-trace-id", model.RequestId);
    }
    
    [Fact]
    public void Error_WithCurrentActivityId_ReturnsViewResultWithActivityIdAsRequestId()
    {
        // Arrange
        var activity = new Activity("TestActivity").Start();
        
        // Act
        var result = _controller.Error() as ViewResult;
        
        // Assert
        Assert.NotNull(result);
        var model = Assert.IsType<ErrorViewModel>(result.Model);
        Assert.Equal(activity.Id, model.RequestId);

        // Cleanup
        activity.Stop();
    }
}
