namespace Menuzi.TechnicalTest.Core.Models;

public sealed record DeliveryTime(TimeOnly Start, TimeOnly End, bool IsGreen);
