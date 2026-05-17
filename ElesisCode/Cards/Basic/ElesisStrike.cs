using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Cards;
using Elesis.ElesisCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Cards.Basic;

[Pool(typeof(ElesisCardPool))]
public sealed class ElesisStrike() : ElesisCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2m, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => [ElesisKeywords.Chivalry];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        for (var hit = 0; hit < 3; hit++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);
        }
        await ElesisMechanics.GainChivalry(Owner.Creature, 1m, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
