using Menuzi.TechnicalTest.Api.Models;
using Menuzi.TechnicalTest.Core.Interfaces;
using Menuzi.TechnicalTest.Core.Models;
using Menuzi.TechnicalTest.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Menuzi.TechnicalTest.Api.Controllers;

[ApiController]
[Route("api/delivery-slots")]
public sealed class DeliverySlotsController(
    IProductCatalog catalog,
    DeliverySlotService deliverySlotService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<DeliverySlotsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DeliverySlotsResponse> Get([FromQuery] string[]? productIds)
    {
        if (productIds is null || productIds.Length == 0)
        {
            ModelState.AddModelError(
                nameof(productIds),
                "At least one product id is required.");

            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var products = new List<Product>();
        var unknownIds = new List<string>();

        foreach (var id in productIds
                     .Where(id => !string.IsNullOrWhiteSpace(id))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (catalog.TryGetById(id, out var product))
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
                nameof(productIds),
                $"Unknown product id(s): {string.Join(", ", unknownIds)}.");

            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        if (products.Count == 0)
        {
            ModelState.AddModelError(
                nameof(productIds),
                "At least one valid product id is required.");

            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var slots = deliverySlotService.GetSlots(products);
        var response = new DeliverySlotsResponse(
            slots.Select(DeliverySlotResponse.From).ToList());

        return Ok(response);
    }
}
