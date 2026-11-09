using Menuzi.TechnicalTest.Core.Interfaces;

namespace Menuzi.TechnicalTest.Data;

public sealed class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}
