using Menuzi.TechnicalTest.Api.Models;
using Menuzi.TechnicalTest.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Menuzi.TechnicalTest.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductCatalog catalog) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<ProductResponse>> Get()
    {
        var products = catalog.GetAll()
            .Select(ProductResponse.From)
            .ToList();

        return Ok(products);
    }
}
