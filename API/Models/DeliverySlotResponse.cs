using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Api.Models;

public sealed record DeliverySlotResponse(
    string Date,
    IReadOnlyList<DeliveryTimeResponse> Times)
{
    public static DeliverySlotResponse From(DeliverySlot slot)
    {
        return new DeliverySlotResponse(
            slot.Date.ToString("yyyy-MM-dd"),
            slot.Times.Select(DeliveryTimeResponse.From).ToList());
    }
}
