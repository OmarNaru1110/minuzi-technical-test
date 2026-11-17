using Menuzi.TechnicalTest.Core.Interfaces;
using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Data;

public sealed class InMemoryCartCatalog : ICartCatalog
{
    private static readonly IReadOnlyList<Cart> Carts =
    [
        new("cart-all",
        [
            new CartItem("warehouse-milk"),
            new CartItem("fresh-sourdough"),
            new CartItem("external-cheddar")
        ]),
        new("cart-warehouse-fresh",
        [
            new CartItem("warehouse-eggs"),
            new CartItem("fresh-lasagna")
        ]),
        new("cart-warehouse-external",
        [
            new CartItem("warehouse-water"),
            new CartItem("external-truffle")
        ]),
        new("cart-fresh-external",
        [
            new CartItem("fresh-croissants"),
            new CartItem("external-cheddar")
        ]),
        new("cart-warehouse",
        [
            new CartItem("warehouse-milk"),
            new CartItem("warehouse-eggs")
        ]),
        new("cart-fresh", [new CartItem("fresh-sourdough")]),
        new("cart-external", [new CartItem("external-cheddar")]),
        new("cart-empty", [])
    ];

    public IReadOnlyList<Cart> GetAll() => Carts;

    public bool TryGetById(string id, out Cart cart)
    {
        cart = Carts.SingleOrDefault(c => c.Id == id);
        return cart is not null;
    }
}
