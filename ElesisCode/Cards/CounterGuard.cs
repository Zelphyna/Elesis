using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Elesis.ElesisCode.Cards;

[Pool(typeof(ElesisCardPool))]
public sealed class CounterGuard : ElesisCard
{
    private decimal _counterAttack = 6m;

    public CounterGuard() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainCounterAttack(choiceContext, Owner.Creature, _counterAttack, this);
    }

    protected override void OnUpgrade()
    {
        _counterAttack += 3m;
    }
}
