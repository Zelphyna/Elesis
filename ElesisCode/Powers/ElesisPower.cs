using BaseLib.Abstracts;
using BaseLib.Extensions;
using Elesis.ElesisCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Elesis.ElesisCode.Powers;

public abstract class ElesisPower : CustomPowerModel
{
    public override string CustomPackedIconPath => "power.png".PowerImagePath();
    public override string CustomBigIconPath => "power.png".BigPowerImagePath();

    public void TriggerFlash() => Flash();

    public abstract override PowerType Type { get; }
    public abstract override PowerStackType StackType { get; }
}
