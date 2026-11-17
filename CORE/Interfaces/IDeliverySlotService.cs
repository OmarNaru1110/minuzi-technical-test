using Menuzi.TechnicalTest.Core.Models;

namespace Menuzi.TechnicalTest.Core.Interfaces;

public interface IDeliverySlotService
{
    IReadOnlyList<DeliverySlot> GetSlots(IReadOnlyCollection<Product> products);
}
