using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Elesis.ElesisCode.Cards;

[Pool(typeof(ElesisCardPool))]
public sealed class BurningEdge : ElesisCard
{
    private decimal _burn = 5m;

    public BurningEdge() : base(1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.ApplyBurn(choiceContext, cardPlay.Target, _burn, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        _burn += 3m;
    }
}
