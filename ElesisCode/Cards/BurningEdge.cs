using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using Elesis.ElesisCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Elesis.ElesisCode.Cards;

[Pool(typeof(ElesisCardPool))]
public sealed class BurningEdge : ElesisCard
{
    public BurningEdge() : base(1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BurnPower>(5)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.ApplyBurn(choiceContext, cardPlay.Target,
            DynamicVars[nameof(BurnPower)].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(3);
    }
}
