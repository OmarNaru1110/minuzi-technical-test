using Menuzi.TechnicalTest.Api.Controllers;
using Menuzi.TechnicalTest.Api.Models;
using Menuzi.TechnicalTest.Core.Interfaces;
using Menuzi.TechnicalTest.Core.Models;
using Menuzi.TechnicalTest.Core.Services;
using Menuzi.TechnicalTest.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Menuzi.TechnicalTest.Tests;

public sealed class CartsControllerTests
{
    private static readonly DateTime Now = new(2026, 11, 9, 10, 30, 0);

    [Fact]
    public void Get_ReturnsSeededCarts()
    {
        var controller = CreateController();

        var result = controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var carts = Assert.IsAssignableFrom<IReadOnlyList<CartResponse>>(ok.Value);

        Assert.Equal(8, carts.Count);
        Assert.Contains(carts, cart => cart.Id == "cart-all" && cart.Items.Count == 3);
        Assert.Contains(carts, cart => cart.Id == "cart-empty" && cart.Items.Count == 0);
    }

    [Fact]
    public void GetDeliverySlots_WithAllProductTypesCart_ReturnsSlots()
    {
        var controller = CreateController();

        var result = controller.GetDeliverySlots("cart-all");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<DeliverySlotsResponse>(ok.Value);

        Assert.Equal(6, response.Slots.Count);
        Assert.Equal(84, response.Slots.Sum(slot => slot.Times.Count));
        Assert.Contains(response.Slots, slot => slot.Times.Any(time => time.IsGreen));
        Assert.Equal("2026-11-12", response.Slots[0].Date);
        Assert.Equal("13:00", response.Slots[0].Times[0].StartTime);
    }

    [Fact]
    public void GetDeliverySlots_WithWarehouseOnlyCart_ReturnsSameDaySlots()
    {
        var controller = CreateController();

        var result = controller.GetDeliverySlots("cart-warehouse");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<DeliverySlotsResponse>(ok.Value);

        Assert.Equal(10, response.Slots.Count);
        Assert.Equal(137, response.Slots.Sum(slot => slot.Times.Count));
        Assert.Equal("2026-11-09", response.Slots[0].Date);
    }

    [Fact]
    public void GetDeliverySlots_WithUnknownCartId_ReturnsNotFound()
    {
        var controller = CreateController();

        var result = controller.GetDeliverySlots("does-not-exist");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetDeliverySlots_WithEmptyCart_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = controller.GetDeliverySlots("cart-empty");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public void GetDeliverySlots_WithCartContainingUnknownProduct_ReturnsBadRequest()
    {
        var controller = new CartsController(
            new StubCartCatalog(
                new Cart("cart-bad", [new CartItem("does-not-exist")])),
            new InMemoryProductCatalog(),
            new DeliverySlotService(new FixedClock(Now)));

        var result = controller.GetDeliverySlots("cart-bad");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    private static CartsController CreateController()
    {
        IDeliverySlotService deliverySlots = new DeliverySlotService(new FixedClock(Now));

        return new CartsController(
            new InMemoryCartCatalog(),
            new InMemoryProductCatalog(),
            deliverySlots);
    }

    private sealed class FixedClock(DateTime now) : IClock
    {
        public DateTime Now { get; } = now;
    }

    private sealed class StubCartCatalog(Cart cart) : ICartCatalog
    {
        public IReadOnlyList<Cart> GetAll() => [cart];

        public bool TryGetById(string id, out Cart result)
        {
            if (cart.Id == id)
            {
                result = cart;
                return true;
            }

            result = null!;
            return false;
        }
    }
}
