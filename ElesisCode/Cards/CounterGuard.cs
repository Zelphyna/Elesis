using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using Elesis.ElesisCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Elesis.ElesisCode.Cards;

[Pool(typeof(ElesisCardPool))]
public sealed class CounterGuard : ElesisCard
{
    public CounterGuard() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CounterAttackPower>(6)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainCounterAttack(choiceContext, Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(3);
    }
}
