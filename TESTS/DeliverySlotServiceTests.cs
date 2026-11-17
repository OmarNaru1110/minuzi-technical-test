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

        Assert.Equal(84, slots.Sum(slot => slot.Times.Count));
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
            slots.Select(slot => slot.Date).ToArray());
        Assert.All(
            slots,
            slot => Assert.True(slot.Date.DayOfWeek is >= DayOfWeek.Tuesday and <= DayOfWeek.Friday));
    }

    [Fact]
    public void WarehouseOnly_OnMondayAt1030_ExcludesPastSameDaySlots()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });
        var today = new DateOnly(2026, 11, 9);
        var todayTimes = slots.Single(slot => slot.Date == today).Times;

        Assert.Equal(151, slots.Sum(slot => slot.Times.Count));
        Assert.Equal(11, todayTimes.Count);
        Assert.Equal(new TimeOnly(11, 0), todayTimes.Min(time => time.Start));
        Assert.DoesNotContain(todayTimes, time => time.Start <= new TimeOnly(10, 30));
    }

    [Fact]
    public void WarehouseAndFreshFood_OnMondayAt1030_AllowSameDayDelivery()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse, FreshFood });

        Assert.Equal(new DateOnly(2026, 11, 9), slots[0].Date);
        Assert.Equal(151, slots.Sum(slot => slot.Times.Count));
    }

    [Fact]
    public void WarehouseOnly_OnFridayAt1730_AllowsSameDaySlotsAfterNow()
    {
        var now = new DateTime(2026, 11, 13, 17, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });
        var today = new DateOnly(2026, 11, 13);
        var todayTimes = slots.Single(slot => slot.Date == today).Times;

        Assert.Equal(144, slots.Sum(slot => slot.Times.Count));
        Assert.Equal(4, todayTimes.Count);
        Assert.Equal(
            new[] { 18, 19, 20, 21 },
            todayTimes.Select(time => time.Start.Hour).OrderBy(hour => hour).ToArray());
    }

    [Fact]
    public void WarehouseOnly_AtExactCutoff_ExcludesSameDay()
    {
        var now = new DateTime(2026, 11, 13, 18, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });

        Assert.Equal(140, slots.Sum(slot => slot.Times.Count));
        Assert.DoesNotContain(slots, slot => slot.Date == new DateOnly(2026, 11, 13));
        Assert.Equal(new DateOnly(2026, 11, 16), slots[0].Date);
    }

    [Fact]
    public void FreshFood_AtExactCutoff_ExcludesSameDay()
    {
        var now = new DateTime(2026, 11, 9, 12, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { FreshFood });

        Assert.Equal(140, slots.Sum(slot => slot.Times.Count));
        Assert.DoesNotContain(slots, slot => slot.Date == new DateOnly(2026, 11, 9));
        Assert.Equal(new DateOnly(2026, 11, 10), slots[0].Date);
    }

    [Fact]
    public void FreshFood_BeforeCutoff_AllowsSameDaySlotsFromNoon()
    {
        var now = new DateTime(2026, 11, 9, 11, 59, 0);

        var slots = ServiceAt(now).GetSlots(new[] { FreshFood });
        var today = new DateOnly(2026, 11, 9);
        var todayTimes = slots.Single(slot => slot.Date == today).Times;

        Assert.Equal(150, slots.Sum(slot => slot.Times.Count));
        Assert.Equal(10, todayTimes.Count);
        Assert.Equal(new TimeOnly(12, 0), todayTimes.Min(time => time.Start));
    }

    [Fact]
    public void External_OnThursday_StartsNextTuesday()
    {
        var now = new DateTime(2026, 11, 12, 9, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { External });

        Assert.Equal(98, slots.Sum(slot => slot.Times.Count));
        Assert.Equal(new DateOnly(2026, 11, 17), slots[0].Date);
        Assert.Equal(7, slots.Count);
    }

    [Fact]
    public void External_OnFriday_StartsNextTuesday()
    {
        var now = new DateTime(2026, 11, 13, 9, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { External });

        Assert.Equal(112, slots.Sum(slot => slot.Times.Count));
        Assert.Equal(new DateOnly(2026, 11, 17), slots[0].Date);
        Assert.Equal(8, slots.Count);
    }

    [Fact]
    public void External_OnSaturday_StartsNextTuesday()
    {
        var now = new DateTime(2026, 11, 14, 9, 0, 0);

        var slots = ServiceAt(now).GetSlots(new[] { External });

        Assert.Equal(112, slots.Sum(slot => slot.Times.Count));
        Assert.Equal(new DateOnly(2026, 11, 17), slots[0].Date);
        Assert.Equal(8, slots.Count);
    }

    [Fact]
    public void GreenSlots_AreListedFirstWithinEachDay()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });
        var day = slots.Single(slot => slot.Date == new DateOnly(2026, 11, 10)).Times;

        Assert.Equal(
            new[] { 13, 14, 20, 21, 8, 9, 10, 11, 12, 15, 16, 17, 18, 19 },
            day.Select(time => time.Start.Hour).ToArray());
        Assert.Equal(
            new[] { true, true, true, true, false, false, false, false, false, false, false, false, false, false },
            day.Select(time => time.IsGreen).ToArray());
    }

    [Fact]
    public void Slots_AreGroupedByDate()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse });

        Assert.Equal(11, slots.Count);
        Assert.Equal(slots.Select(slot => slot.Date).Distinct().Count(), slots.Count);
        Assert.All(slots, slot => Assert.NotEmpty(slot.Times));
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
    public void Times_AreOneHourLongAndWithinOpeningHours()
    {
        var now = new DateTime(2026, 11, 9, 10, 30, 0);

        var slots = ServiceAt(now).GetSlots(new[] { Warehouse, FreshFood, External });

        Assert.All(slots, slot => Assert.All(slot.Times, time =>
        {
            Assert.Equal(1, (time.End - time.Start).TotalHours);
            Assert.InRange(time.Start.Hour, 8, 21);
            Assert.Equal(time.Start.Hour + 1, time.End.Hour);
        }));
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
