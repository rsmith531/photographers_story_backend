using Microsoft.AspNetCore.Mvc;

namespace Tests;

public class HealthControllerTests
{
    [Fact]
    public void HealthCheck_Returns204_ForGetRequest()
    {
        // Arrange
        var controller = new Api.Controllers.HealthController();
        
        // Act
        var result = controller.Get();

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);

    }
}
