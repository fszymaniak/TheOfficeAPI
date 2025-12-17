using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using TheOfficeAPI.Common.Middleware;
using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Common.Tests.Unit;

public class GlobalExceptionHandlerMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionHandlerMiddleware>> _loggerMock;
    private readonly Mock<IHostEnvironment> _environmentMock;
    private readonly DefaultHttpContext _httpContext;

    public GlobalExceptionHandlerMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandlerMiddleware>>();
        _environmentMock = new Mock<IHostEnvironment>();
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNext()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = (HttpContext _) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_ArgumentNullException_ReturnsBadRequest()
    {
        // Arrange
        RequestDelegate next = (HttpContext _) => throw new ArgumentNullException("testParam");
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(400, _httpContext.Response.StatusCode);
        Assert.Equal("application/json", _httpContext.Response.ContentType);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(400, errorResponse.StatusCode);
        Assert.Equal("Invalid request parameters", errorResponse.Message);
    }

    [Fact]
    public async Task InvokeAsync_ArgumentException_ReturnsBadRequest()
    {
        // Arrange
        RequestDelegate next = (HttpContext _) => throw new ArgumentException("Invalid argument");
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(400, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(400, errorResponse.StatusCode);
        Assert.Equal("Invalid request parameters", errorResponse.Message);
        Assert.Equal("Invalid argument", errorResponse.Details);
    }

    [Fact]
    public async Task InvokeAsync_KeyNotFoundException_ReturnsNotFound()
    {
        // Arrange
        RequestDelegate next = (HttpContext _) => throw new KeyNotFoundException("Resource not found");
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(404, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(404, errorResponse.StatusCode);
        Assert.Equal("The requested resource was not found", errorResponse.Message);
        Assert.Equal("Resource not found", errorResponse.Details);
    }

    [Fact]
    public async Task InvokeAsync_UnauthorizedAccessException_ReturnsUnauthorized()
    {
        // Arrange
        RequestDelegate next = (HttpContext _) => throw new UnauthorizedAccessException();
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(401, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(401, errorResponse.StatusCode);
        Assert.Equal("Unauthorized access", errorResponse.Message);
    }

    [Fact]
    public async Task InvokeAsync_InvalidOperationException_ReturnsConflict()
    {
        // Arrange
        RequestDelegate next = (HttpContext _) => throw new InvalidOperationException("Invalid state");
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(409, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(409, errorResponse.StatusCode);
        Assert.Equal("Operation cannot be completed in the current state", errorResponse.Message);
        Assert.Equal("Invalid state", errorResponse.Details);
    }

    [Fact]
    public async Task InvokeAsync_NotImplementedException_ReturnsNotImplemented()
    {
        // Arrange
        RequestDelegate next = (HttpContext _) => throw new NotImplementedException();
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(501, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(501, errorResponse.StatusCode);
        Assert.Equal("This functionality is not yet implemented", errorResponse.Message);
    }

    [Fact]
    public async Task InvokeAsync_TimeoutException_ReturnsRequestTimeout()
    {
        // Arrange
        RequestDelegate next = (HttpContext _) => throw new TimeoutException();
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(408, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(408, errorResponse.StatusCode);
        Assert.Equal("The request timed out", errorResponse.Message);
    }

    [Fact]
    public async Task InvokeAsync_GenericException_ReturnsInternalServerError()
    {
        // Arrange
        RequestDelegate next = (HttpContext _) => throw new Exception("Something went wrong");
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal(500, _httpContext.Response.StatusCode);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(500, errorResponse.StatusCode);
        Assert.Equal("An internal server error occurred", errorResponse.Message);
        Assert.Null(errorResponse.Details); // No details in production
    }

    [Fact]
    public async Task InvokeAsync_DevelopmentEnvironment_IncludesDetailedErrors()
    {
        // Arrange
        RequestDelegate next = (HttpContext _) => throw new Exception("Detailed error");
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.NotNull(errorResponse.Details);
        Assert.Contains("Detailed error", errorResponse.Details);
    }

    [Fact]
    public async Task InvokeAsync_SetsTraceIdAndPath()
    {
        // Arrange
        _httpContext.Request.Path = "/api/test";
        _httpContext.TraceIdentifier = "test-trace-id";

        RequestDelegate next = (HttpContext _) => throw new Exception("Test");
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal("/api/test", errorResponse.Path);
        Assert.Equal("test-trace-id", errorResponse.TraceId);
    }

    [Fact]
    public async Task InvokeAsync_LogsException()
    {
        // Arrange
        var exception = new Exception("Test exception");
        RequestDelegate next = (HttpContext _) => throw exception;
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
