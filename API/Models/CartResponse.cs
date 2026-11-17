using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Api.Models;

public sealed record CartResponse(
    string Id,
    IReadOnlyList<CartItemResponse> Items)
{
    public static CartResponse From(Cart cart)
    {
        return new CartResponse(
            cart.Id,
            cart.Items.Select(CartItemResponse.From).ToList());
    }
}
