using TheOfficeAPI.Common.Models;

namespace TheOfficeAPI.Common.Tests.Unit;

public class ErrorResponseTests
{
    [Fact]
    public void ErrorResponse_DefaultConstructor_SetsDefaultValues()
    {
        // Act
        var errorResponse = new ErrorResponse();

        // Assert
        Assert.Equal(0, errorResponse.StatusCode);
        Assert.Equal(string.Empty, errorResponse.Message);
        Assert.Null(errorResponse.Details);
        Assert.NotEqual(default(DateTime), errorResponse.Timestamp);
        Assert.Null(errorResponse.Path);
        Assert.Null(errorResponse.TraceId);
    }

    [Fact]
    public void ErrorResponse_TimestampIsSetToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var errorResponse = new ErrorResponse();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(errorResponse.Timestamp >= beforeCreation);
        Assert.True(errorResponse.Timestamp <= afterCreation);
    }

    [Fact]
    public void ErrorResponse_CanSetStatusCode()
    {
        // Arrange
        var errorResponse = new ErrorResponse();

        // Act
        errorResponse.StatusCode = 404;

        // Assert
        Assert.Equal(404, errorResponse.StatusCode);
    }

    [Fact]
    public void ErrorResponse_CanSetMessage()
    {
        // Arrange
        var errorResponse = new ErrorResponse();

        // Act
        errorResponse.Message = "Not Found";

        // Assert
        Assert.Equal("Not Found", errorResponse.Message);
    }

    [Fact]
    public void ErrorResponse_CanSetDetails()
    {
        // Arrange
        var errorResponse = new ErrorResponse();

        // Act
        errorResponse.Details = "Resource with ID 123 was not found";

        // Assert
        Assert.Equal("Resource with ID 123 was not found", errorResponse.Details);
    }

    [Fact]
    public void ErrorResponse_CanSetPath()
    {
        // Arrange
        var errorResponse = new ErrorResponse();

        // Act
        errorResponse.Path = "/api/v1/test";

        // Assert
        Assert.Equal("/api/v1/test", errorResponse.Path);
    }

    [Fact]
    public void ErrorResponse_CanSetTraceId()
    {
        // Arrange
        var errorResponse = new ErrorResponse();

        // Act
        errorResponse.TraceId = "trace-123";

        // Assert
        Assert.Equal("trace-123", errorResponse.TraceId);
    }

    [Fact]
    public void ErrorResponse_CanSetTimestamp()
    {
        // Arrange
        var errorResponse = new ErrorResponse();
        var customTimestamp = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        errorResponse.Timestamp = customTimestamp;

        // Assert
        Assert.Equal(customTimestamp, errorResponse.Timestamp);
    }

    [Fact]
    public void ErrorResponse_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var errorResponse = new ErrorResponse
        {
            StatusCode = 500,
            Message = "Internal Server Error",
            Details = "An unexpected error occurred",
            Path = "/api/v1/test",
            TraceId = "trace-456",
            Timestamp = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc)
        };

        // Assert
        Assert.Equal(500, errorResponse.StatusCode);
        Assert.Equal("Internal Server Error", errorResponse.Message);
        Assert.Equal("An unexpected error occurred", errorResponse.Details);
        Assert.Equal("/api/v1/test", errorResponse.Path);
        Assert.Equal("trace-456", errorResponse.TraceId);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc), errorResponse.Timestamp);
    }
}
