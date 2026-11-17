namespace Menuzi.TechnicalTest.Core.Models;

public sealed record DeliverySlot(DateOnly Date, IReadOnlyList<DeliveryTime> Times);
