namespace Menuzi.TechnicalTest.Core.Models;

public sealed record Cart(string Id, IReadOnlyList<CartItem> Items);
