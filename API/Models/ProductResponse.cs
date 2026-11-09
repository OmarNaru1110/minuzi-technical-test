using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Api.Models;

public sealed record ProductResponse(string Id, string Name, ProductType Type)
{
    public static ProductResponse From(Product product)
    {
        return new ProductResponse(product.Id, product.Name, product.Type);
    }
}
