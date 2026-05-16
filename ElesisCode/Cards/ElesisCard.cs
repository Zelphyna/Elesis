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
    public override IEnumerable<CardKeyword> CanonicalKeywords => ElesisCardKeywords;

    public override string CustomPortraitPath => CardImageFileName.BigCardImagePath();
    public override string PortraitPath => CardImageFileName.CardImagePath();
    public override string BetaPortraitPath => CardImageFileName.CardImagePath();

    protected virtual IEnumerable<CardKeyword> ElesisCardKeywords => [];

    private string CardImageFileName => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
}
