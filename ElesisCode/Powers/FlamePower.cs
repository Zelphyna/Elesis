using MegaCrit.Sts2.Core.Entities.Powers;
using Elesis.ElesisCode.Extensions;

namespace Elesis.ElesisCode.Powers;

public sealed class FlamePower : ElesisPower
{
    public override string CustomPackedIconPath => "flame_power.png".PowerImagePath();
    public override string CustomBigIconPath => "flame_power.png".BigPowerImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
