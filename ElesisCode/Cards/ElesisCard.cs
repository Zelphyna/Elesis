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
    // Prototype card art is shared until each Elesis card has dedicated art.
    public override string CustomPortraitPath => "card.png".BigCardImagePath();
    public override string PortraitPath => "card.png".CardImagePath();
    public override string BetaPortraitPath => "card.png".CardImagePath();
}
