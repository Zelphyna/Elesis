using MegaCrit.Sts2.Core.Entities.Powers;

namespace Elesis.ElesisCode.Powers;

public sealed class FlamePower : ElesisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
