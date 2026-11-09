using Menuzi.TechnicalTest.Core.Interfaces;
using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Data;

public sealed class InMemoryProductCatalog : IProductCatalog
{
    private static readonly IReadOnlyList<Product> Products =
    [
        new("warehouse-milk", "Organic Milk", ProductType.Warehouse),
        new("warehouse-eggs", "Free-Range Eggs", ProductType.Warehouse),
        new("warehouse-water", "Sparkling Water", ProductType.Warehouse),
        new("fresh-sourdough", "Sourdough Bread", ProductType.FreshFood),
        new("fresh-lasagna", "Beef Lasagna", ProductType.FreshFood),
        new("fresh-croissants", "Butter Croissants", ProductType.FreshFood),
        new("external-cheddar", "Aged Cheddar", ProductType.External),
        new("external-truffle", "Truffle Oil", ProductType.External)
    ];

    public IReadOnlyList<Product> GetAll() => Products;

    public bool TryGetById(string id, out Product product)
    {
        foreach (var p in Products)
        {
            if (p.Id == id)
            {
                product = p;
                return true;
            }
        }

        product = null!;
        return false;
    }
}
