using MegaCrit.Sts2.Core.Entities.Powers;

namespace Elesis.ElesisCode.Powers;

public sealed class ChivalryPower : ElesisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
