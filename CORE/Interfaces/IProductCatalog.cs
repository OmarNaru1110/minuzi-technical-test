using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Core.Interfaces;

public interface IProductCatalog
{
    IReadOnlyList<Product> GetAll();

    bool TryGetById(string id, out Product product);
}
