using Menuzi.TechnicalTest.Core.Interfaces;
using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Core.Services;

public sealed class DeliverySlotService(IClock clock)
{
    public const int SchedulingWindowDays = 14;

    private const int ExternalLeadDays = 3;
    private const int FirstSlotHour = 8;
    private const int LastSlotHour = 22;

    private static readonly TimeSpan WarehouseCutoff = TimeSpan.FromHours(18);
    private static readonly TimeSpan FreshFoodCutoff = TimeSpan.FromHours(12);

    private static readonly (TimeOnly Start, TimeOnly End)[] GreenRanges =
    [
        (new TimeOnly(13, 0), new TimeOnly(15, 0)),
        (new TimeOnly(20, 0), new TimeOnly(22, 0))
    ];

    public IReadOnlyList<DeliverySlot> GetSlots(IReadOnlyCollection<Product> products)
    {
        if (products.Count == 0)
        {
            return Array.Empty<DeliverySlot>();
        }

        var now = clock.Now;
        var today = DateOnly.FromDateTime(now);
        var lastDate = today.AddDays(SchedulingWindowDays);
        var currentTime = TimeOnly.FromDateTime(now);
        var slots = new List<DeliverySlot>();

        for (var date = today; date <= lastDate; date = date.AddDays(1))
        {
            if (!IsWeekday(date) || !AllProductsEligible(products, date, now))
            {
                continue;
            }

            for (var hour = FirstSlotHour; hour < LastSlotHour; hour++)
            {
                var start = new TimeOnly(hour, 0);

                if (date == today && start <= currentTime)
                {
                    continue;
                }

                slots.Add(new DeliverySlot(
                    date,
                    start,
                    start.Add(TimeSpan.FromHours(1)),
                    IsGreen(start)));
            }
        }

        return slots
            .OrderBy(slot => slot.Date)
            .ThenByDescending(slot => slot.IsGreen)
            .ThenBy(slot => slot.Start)
            .ToList();
    }

    private static bool AllProductsEligible(
        IReadOnlyCollection<Product> products,
        DateOnly date,
        DateTime now)
    {
        return products.All(product => IsEligible(product, date, now));
    }

    // now is useful to if the date is today, we can check if the current time is before the cutoff time for the product type   
    private static bool IsEligible(Product product, DateOnly date, DateTime now)
    {
        var today = DateOnly.FromDateTime(now);

        if (date < today)
        {
            return false;
        }

        return product.Type switch
        {
            ProductType.Warehouse => date > today || now.TimeOfDay < WarehouseCutoff,
            ProductType.FreshFood => date > today || now.TimeOfDay < FreshFoodCutoff,
            ProductType.External => date >= today.AddDays(ExternalLeadDays) && IsExternalDeliveryDay(date),
            _ => false
        };
    }

    private static bool IsWeekday(DateOnly date)
    {
        return date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday;
    }

    private static bool IsExternalDeliveryDay(DateOnly date)
    {
        return date.DayOfWeek is >= DayOfWeek.Tuesday and <= DayOfWeek.Friday;
    }

    private static bool IsGreen(TimeOnly start)
    {
        return GreenRanges.Any(range => start >= range.Start && start < range.End);
    }
}
