using Menuzi.TechnicalTest.Core.Interfaces;
using Menuzi.TechnicalTest.Core.Models;
using Menuzi.TechnicalTest.Core.Services;

namespace Menuzi.TechnicalTest.Tests;

public sealed class DeliverySlotServiceTests
{
    private static readonly Product Warehouse =
        new("warehouse", "Warehouse", ProductType.Warehouse);

    private static readonly Product FreshFood =
        new("fresh", "Fresh Food", ProductType.FreshFood);

    private static readonly Product External =
        new("external", "External", ProductType.External);

    private static DeliverySlotService ServiceAt(DateTime now)
    {
        return new DeliverySlotService(new FixedClock(now));
    }

    [Fact]
    public void AllCategories_OnMondayAt1030_ReturnsExampleSchedule()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse, FreshFood, External });
        var dates = slots.Select(slot => slot.Date).Distinct().ToList();

        Assert.Equal(84, slots.Count);
        Assert.Equal(
            new[]
            {
                new DateOnly(2026, 11, 12),
                new DateOnly(2026, 11, 13),
                new DateOnly(2026, 11, 17),
                new DateOnly(2026, 11, 18),
                new DateOnly(2026, 11, 19),
                new DateOnly(2026, 11, 20)
            },
            dates);
        Assert.All(
            dates,
            date => Assert.True(date.DayOfWeek is >= DayOfWeek.Tuesday and <= DayOfWeek.Friday));
    }

    [Fact]
    public void WarehouseOnly_OnMondayAt1030_ExcludesPastSameDaySlots()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });
        var today = new DateOnly(2026, 11, 9);
        var todaySlots = slots.Where(slot => slot.Date == today).ToList();

        Assert.Equal(151, slots.Count);
        Assert.Equal(11, todaySlots.Count);
        Assert.Equal(new TimeOnly(11, 0), todaySlots.Min(slot => slot.Start));
        Assert.DoesNotContain(todaySlots, slot => slot.Start <= new TimeOnly(10, 30));
    }

    [Fact]
    public void WarehouseAndFreshFood_OnMondayAt1030_AllowSameDayDelivery()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse, FreshFood });

        Assert.Equal(new DateOnly(2026, 11, 9), slots[0].Date);
        Assert.Equal(151, slots.Count);
    }

    [Fact]
    public void WarehouseOnly_OnFridayAt1730_AllowsSameDaySlotsAfterNow()
    {
        var now = new DateTime(2026, 11, 13, 17, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });
        var today = new DateOnly(2026, 11, 13);
        var todaySlots = slots.Where(slot => slot.Date == today).ToList();

        Assert.Equal(144, slots.Count);
        Assert.Equal(4, todaySlots.Count);
        Assert.Equal(
            new[] { 18, 19, 20, 21 },
            todaySlots.Select(slot => slot.Start.Hour).OrderBy(hour => hour).ToArray());
    }

    [Fact]
    public void WarehouseOnly_AtExactCutoff_ExcludesSameDay()
    {
        var now = new DateTime(2026, 11, 13, 18, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });

        Assert.Equal(140, slots.Count);
        Assert.DoesNotContain(slots, slot => slot.Date == new DateOnly(2026, 11, 13));
        Assert.Equal(new DateOnly(2026, 11, 16), slots[0].Date);
    }

    [Fact]
    public void FreshFood_AtExactCutoff_ExcludesSameDay()
    {
        var now = new DateTime(2026, 11, 9, 12, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { FreshFood });

        Assert.Equal(140, slots.Count);
        Assert.DoesNotContain(slots, slot => slot.Date == new DateOnly(2026, 11, 9));
        Assert.Equal(new DateOnly(2026, 11, 10), slots[0].Date);
    }

    [Fact]
    public void FreshFood_BeforeCutoff_AllowsSameDaySlotsFromNoon()
    {
        var now = new DateTime(2026, 11, 9, 11, 59, 0);

        var slots = ServiceAt(now).GetSlots(new[] { FreshFood });
        var today = new DateOnly(2026, 11, 9);
        var todaySlots = slots.Where(slot => slot.Date == today).ToList();

        Assert.Equal(150, slots.Count);
        Assert.Equal(10, todaySlots.Count);
        Assert.Equal(new TimeOnly(12, 0), todaySlots.Min(slot => slot.Start));
    }

    [Fact]
    public void External_OnThursday_StartsNextTuesday()
    {
        var now = new DateTime(2026, 11, 12, 9, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { External });

        Assert.Equal(98, slots.Count);
        Assert.Equal(new DateOnly(2026, 11, 17), slots[0].Date);
        Assert.Equal(7, slots.Select(slot => slot.Date).Distinct().Count());
    }

    [Fact]
    public void External_OnFriday_StartsNextTuesday()
    {
        var now = new DateTime(2026, 11, 13, 9, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { External });

        Assert.Equal(112, slots.Count);
        Assert.Equal(new DateOnly(2026, 11, 17), slots[0].Date);
        Assert.Equal(8, slots.Select(slot => slot.Date).Distinct().Count());
    }

    [Fact]
    public void External_OnSaturday_StartsNextTuesday()
    {
        var now = new DateTime(2026, 11, 14, 9, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { External });

        Assert.Equal(112, slots.Count);
        Assert.Equal(new DateOnly(2026, 11, 17), slots[0].Date);
        Assert.Equal(8, slots.Select(slot => slot.Date).Distinct().Count());
    }

    [Fact]
    public void GreenSlots_AreListedFirstWithinEachDay()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });
        var day = slots.Where(slot => slot.Date == new DateOnly(2026, 11, 10)).ToList();

        Assert.Equal(
            new[] { 13, 14, 20, 21, 8, 9, 10, 11, 12, 15, 16, 17, 18, 19 },
            day.Select(slot => slot.Start.Hour).ToArray());
        Assert.Equal(
            new[] { true, true, true, true, false, false, false, false, false, false, false, false, false, false },
            day.Select(slot => slot.IsGreen).ToArray());
    }

    [Fact]
    public void Window_IncludesDay14ButNotDay15()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });

        Assert.Contains(slots, slot => slot.Date == new DateOnly(2026, 11, 23));
        Assert.DoesNotContain(slots, slot => slot.Date == new DateOnly(2026, 11, 24));
    }

    [Fact]
    public void Weekends_AreNeverReturned()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse, FreshFood, External });

        Assert.All(
            slots,
            slot => Assert.True(slot.Date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday));
    }

    [Fact]
    public void Slots_AreOneHourLongAndWithinOpeningHours()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse, FreshFood, External });

        Assert.All(slots, slot =>
        {
            Assert.Equal(1, (slot.End - slot.Start).TotalHours);
            Assert.InRange(slot.Start.Hour, 8, 21);
            Assert.Equal(slot.Start.Hour + 1, slot.End.Hour);
        });
    }

    [Fact]
    public void EmptyCart_ReturnsNoSlots()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(Array.Empty<Product>());

        Assert.Empty(slots);
    }

    private sealed class FixedClock(DateTime now) : IClock
    {
        public DateTime Now { get; } = now;
    }
}
