using Menuzi.TechnicalTest.Api.Models;
using Menuzi.TechnicalTest.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Menuzi.TechnicalTest.Api.Controllers;

[ApiController]
[Route("api/carts")]
public sealed class CartsController(ICartCatalog cartCatalog) : ControllerBase
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
}
