using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using Elesis.ElesisCode.Extensions;

namespace Elesis.ElesisCode.Relics;

[Pool(typeof(ElesisRelicPool))]
public abstract class ElesisRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
