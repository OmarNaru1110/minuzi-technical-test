using Menuzi.TechnicalTest.Api.Models;
using Menuzi.TechnicalTest.Core.Interfaces;
using Menuzi.TechnicalTest.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Menuzi.TechnicalTest.Api.Controllers;

[ApiController]
[Route("api/carts")]
public sealed class CartsController(
    ICartCatalog cartCatalog,
    IProductCatalog productCatalog,
    IDeliverySlotService deliverySlotService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CartResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<CartResponse>> Get()
    {
        var carts = cartCatalog.GetAll()
            .Select(CartResponse.From)
            .ToList();

        return Ok(carts);
    }

    [HttpGet("{cartId}/delivery-slots")]
    [ProducesResponseType<DeliverySlotsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DeliverySlotsResponse> GetDeliverySlots([FromRoute] string cartId)
    {
        if (!cartCatalog.TryGetById(cartId, out var cart))
        {
            return NotFound();
        }

        if (cart.Items.Count == 0)
        {
            ModelState.AddModelError(nameof(cartId), "The cart is empty.");

            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var products = new List<Product>();
        var unknownIds = new List<string>();

        foreach (var id in cart.Items
                     .Select(item => item.ProductId)
                     .Where(id => !string.IsNullOrWhiteSpace(id))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (productCatalog.TryGetById(id, out var product))
            {
                products.Add(product);
            }
            else
            {
                unknownIds.Add(id);
            }
        }

        if (unknownIds.Count > 0)
        {
            ModelState.AddModelError(
                nameof(cartId),
                $"Unknown product id(s): {string.Join(", ", unknownIds)}.");

            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        if (products.Count == 0)
        {
            ModelState.AddModelError(nameof(cartId), "The cart has no valid products.");

            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var slots = deliverySlotService.GetSlots(products);
        var response = new DeliverySlotsResponse(
            slots.Select(DeliverySlotResponse.From).ToList());

        return Ok(response);
    }
}
