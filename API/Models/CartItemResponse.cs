using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Api.Models;

public sealed record CartItemResponse(string ProductId)
{
    public static CartItemResponse From(CartItem item)
    {
        return new CartItemResponse(item.ProductId);
    }
}
