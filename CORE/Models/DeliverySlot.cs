namespace Menuzi.TechnicalTest.Core.Models;

public sealed record DeliverySlot(DateOnly Date, TimeOnly Start, TimeOnly End, bool IsGreen);
