using Menuzi.TechnicalTest.Api.Controllers;
using Menuzi.TechnicalTest.Api.Models;
using Menuzi.TechnicalTest.Core.Interfaces;
using Menuzi.TechnicalTest.Core.Services;
using Menuzi.TechnicalTest.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Menuzi.TechnicalTest.Tests;

public sealed class DeliverySlotsControllerTests
{
    private static readonly DateTime Now = new(2026, 11, 9, 10, 30, 0);

    [Fact]
    public void Get_WithKnownProducts_ReturnsSlots()
    {
        var controller = CreateController();

        var result = controller.Get(new[]
        {
            "warehouse-milk",
            "fresh-sourdough",
            "external-cheddar"
        });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<DeliverySlotsResponse>(ok.Value);

        Assert.Equal(84, response.Slots.Count);
        Assert.Contains(response.Slots, slot => slot.IsGreen);
    }

    [Fact]
    public void Get_WithEmptyProductIds_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = controller.Get(Array.Empty<string>());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public void Get_WithUnknownProductId_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = controller.Get(new[] { "does-not-exist" });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    private static DeliverySlotsController CreateController()
    {
        return new DeliverySlotsController(
            new InMemoryProductCatalog(),
            new DeliverySlotService(new FixedClock(Now)));
    }

    private sealed class FixedClock(DateTime now) : IClock
    {
        public DateTime Now { get; } = now;
    }
}
