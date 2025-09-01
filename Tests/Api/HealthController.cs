using Microsoft.AspNetCore.Mvc;

namespace Tests.Api;

public class HealthControllerTests
{
    [Fact]
    public void HealthCheck_Returns204_ForGetRequest()
    {
        // Arrange
        // avoid the namespace collision with the test namespace
        var controller = new global::Api.Controllers.HealthController();
        
        // Act
        var result = controller.Get();

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);

    }
}
