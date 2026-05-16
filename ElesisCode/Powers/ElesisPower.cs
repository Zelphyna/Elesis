using BaseLib.Abstracts;
using BaseLib.Extensions;
using Elesis.ElesisCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Elesis.ElesisCode.Powers;

public abstract class ElesisPower : CustomPowerModel
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    public abstract override PowerType Type { get; }
    public abstract override PowerStackType StackType { get; }
}
