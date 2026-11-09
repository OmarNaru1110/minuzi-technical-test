using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Api.Models;

public sealed record DeliverySlotResponse(
    string Date,
    string StartTime,
    string EndTime,
    bool IsGreen)
{
    public static DeliverySlotResponse From(DeliverySlot slot)
    {
        return new DeliverySlotResponse(
            slot.Date.ToString("yyyy-MM-dd"),
            slot.Start.ToString("HH:mm"),
            slot.End.ToString("HH:mm"),
            slot.IsGreen);
    }
}
