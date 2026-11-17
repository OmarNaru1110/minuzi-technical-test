using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Core.Interfaces;

public interface ICartCatalog
{
    IReadOnlyList<Cart> GetAll();

    bool TryGetById(string id, out Cart cart);
}
