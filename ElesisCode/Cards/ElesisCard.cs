using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using Elesis.ElesisCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Elesis.ElesisCode.Cards;

[Pool(typeof(ElesisCardPool))]
public abstract class ElesisCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // Normal art: 1000x760. Smaller runtime art: 250x190.
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
