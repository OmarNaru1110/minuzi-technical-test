using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Api.Models;

public sealed record DeliveryTimeResponse(
    string StartTime,
    string EndTime,
    bool IsGreen)
{
    public static DeliveryTimeResponse From(DeliveryTime time)
    {
        return new DeliveryTimeResponse(
            time.Start.ToString("HH:mm"),
            time.End.ToString("HH:mm"),
            time.IsGreen);
    }
}
