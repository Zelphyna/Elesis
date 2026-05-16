using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Cards;

[Pool(typeof(ElesisCardPool))]
public abstract class ElesisAttackCard(
    int cost,
    CardRarity rarity,
    decimal damage,
    decimal upgrade,
    ElesisStyle style = ElesisStyle.None,
    decimal chivalryGain = 0m,
    decimal flameGain = 0m,
    string hitFx = "vfx/vfx_attack_blunt") :
    ElesisCard(cost, CardType.Attack, rarity, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(damage, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.For(style, chivalryGain, flameGain);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonusDamage = 0;
        if (style == ElesisStyle.Destruction && await ElesisMechanics.TrySpendChivalry(Owner.Creature, ElesisMechanics.ChivalryThreshold))
        {
            bonusDamage += 8;
        }

        if (style != ElesisStyle.Flame)
        {
            bonusDamage += await ElesisMechanics.ConsumeFlame(Owner.Creature);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonusDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx(hitFx)
            .Execute(choiceContext);

        await AfterElesisCardPlayed(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(upgrade);
    }

    private async Task AfterElesisCardPlayed(PlayerChoiceContext choiceContext)
    {
        await ElesisMechanics.GainChivalry(Owner.Creature, chivalryGain, this);
        await ElesisMechanics.GainFlame(Owner.Creature, flameGain, this);
        if (style == ElesisStyle.Vitality)
        {
            await ElesisMechanics.ResolveVitalityThreshold(choiceContext, Owner);
        }
    }
}

[Pool(typeof(ElesisCardPool))]
public abstract class ElesisBlockCard(
    int cost,
    CardRarity rarity,
    decimal block,
    decimal upgrade,
    ElesisStyle style = ElesisStyle.None,
    decimal chivalryGain = 0m,
    decimal flameGain = 0m) :
    ElesisCard(cost, CardType.Skill, rarity, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(block, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.For(style, chivalryGain, flameGain);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bonusBlock = 0;
        if (style == ElesisStyle.Destruction && await ElesisMechanics.TrySpendChivalry(Owner.Creature, ElesisMechanics.ChivalryThreshold))
        {
            bonusBlock += 5;
        }

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue + bonusBlock, ValueProp.Move, cardPlay, false);

        await ElesisMechanics.GainChivalry(Owner.Creature, chivalryGain, this);
        await ElesisMechanics.GainFlame(Owner.Creature, flameGain, this);
        if (style == ElesisStyle.Vitality)
        {
            await ElesisMechanics.ResolveVitalityThreshold(choiceContext, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(upgrade);
    }
}

[Pool(typeof(ElesisCardPool))]
public abstract class ElesisAttackBlockCard(
    int cost,
    CardRarity rarity,
    decimal damage,
    decimal block,
    decimal damageUpgrade,
    decimal blockUpgrade,
    ElesisStyle style = ElesisStyle.None,
    decimal chivalryGain = 0m,
    decimal flameGain = 0m,
    string hitFx = "vfx/vfx_attack_blunt") :
    ElesisCard(cost, CardType.Attack, rarity, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(damage, ValueProp.Move),
        new BlockVar(block, ValueProp.Move)
    ];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.For(style, chivalryGain, flameGain);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonusDamage = 0;
        var bonusBlock = 0;
        if (style == ElesisStyle.Destruction && await ElesisMechanics.TrySpendChivalry(Owner.Creature, ElesisMechanics.ChivalryThreshold))
        {
            bonusDamage += 8;
            bonusBlock += 5;
        }

        if (style != ElesisStyle.Flame)
        {
            bonusDamage += await ElesisMechanics.ConsumeFlame(Owner.Creature);
        }

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue + bonusBlock, ValueProp.Move, cardPlay, false);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonusDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx(hitFx)
            .Execute(choiceContext);

        await ElesisMechanics.GainChivalry(Owner.Creature, chivalryGain, this);
        await ElesisMechanics.GainFlame(Owner.Creature, flameGain, this);
        if (style == ElesisStyle.Vitality)
        {
            await ElesisMechanics.ResolveVitalityThreshold(choiceContext, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(damageUpgrade);
        DynamicVars.Block.UpgradeValueBy(blockUpgrade);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class QuickStep() : ElesisBlockCard(0, CardRarity.Common, 3m, 3m, ElesisStyle.Vitality, chivalryGain: 1m);

[Pool(typeof(ElesisCardPool))]
public sealed class RedTempo() : ElesisAttackCard(0, CardRarity.Common, 4m, 3m, ElesisStyle.Vitality, chivalryGain: 1m);

[Pool(typeof(ElesisCardPool))]
public sealed class RisingCut() : ElesisAttackCard(1, CardRarity.Common, 8m, 3m);

[Pool(typeof(ElesisCardPool))]
public sealed class GuardingSlash() : ElesisAttackBlockCard(1, CardRarity.Common, 5m, 5m, 2m, 2m);

[Pool(typeof(ElesisCardPool))]
public sealed class IronFootwork() : ElesisBlockCard(1, CardRarity.Common, 8m, 3m, ElesisStyle.Vitality, chivalryGain: 2m);

[Pool(typeof(ElesisCardPool))]
public sealed class EmberCut() : ElesisAttackCard(1, CardRarity.Common, 7m, 4m, ElesisStyle.Flame, flameGain: 2m);

[Pool(typeof(ElesisCardPool))]
public sealed class SwordPressure() : ElesisAttackCard(1, CardRarity.Common, 9m, 3m);

[Pool(typeof(ElesisCardPool))]
public sealed class ForwardGuard() : ElesisBlockCard(1, CardRarity.Common, 6m, 4m);

[Pool(typeof(ElesisCardPool))]
public sealed class VitalLunge() : ElesisAttackBlockCard(1, CardRarity.Common, 6m, 4m, 3m, 2m, ElesisStyle.Vitality, chivalryGain: 2m);

[Pool(typeof(ElesisCardPool))]
public sealed class FlameTap() : ElesisAttackCard(0, CardRarity.Common, 3m, 4m, ElesisStyle.Flame, flameGain: 1m);

[Pool(typeof(ElesisCardPool))]
public sealed class ClaymoreArc() : ElesisAttackCard(2, CardRarity.Common, 15m, 5m, ElesisStyle.Destruction);

[Pool(typeof(ElesisCardPool))]
public sealed class BelderDiscipline() : ElesisBlockCard(2, CardRarity.Common, 13m, 5m);

[Pool(typeof(ElesisCardPool))]
public sealed class DestructionBlow() : ElesisAttackCard(2, CardRarity.Uncommon, 18m, 7m, ElesisStyle.Destruction);

[Pool(typeof(ElesisCardPool))]
public sealed class VitalityRush() : ElesisAttackBlockCard(1, CardRarity.Uncommon, 7m, 7m, 3m, 3m, ElesisStyle.Vitality, chivalryGain: 2m);

[Pool(typeof(ElesisCardPool))]
public sealed class FlameGuard() : ElesisAttackBlockCard(1, CardRarity.Uncommon, 6m, 8m, 3m, 3m, ElesisStyle.Flame, flameGain: 2m);

[Pool(typeof(ElesisCardPool))]
public sealed class CounterStance() : ElesisBlockCard(1, CardRarity.Uncommon, 10m, 4m, ElesisStyle.Parry, chivalryGain: 1m);

[Pool(typeof(ElesisCardPool))]
public sealed class SpiralBlade() : ElesisAttackCard(2, CardRarity.Uncommon, 21m, 6m, ElesisStyle.Destruction);

[Pool(typeof(ElesisCardPool))]
public sealed class IgnitionEdge() : ElesisAttackCard(1, CardRarity.Uncommon, 10m, 5m, ElesisStyle.Flame, flameGain: 2m);

[Pool(typeof(ElesisCardPool))]
public sealed class BreakingCharge() : ElesisAttackBlockCard(2, CardRarity.Uncommon, 14m, 8m, 5m, 3m, ElesisStyle.Destruction);

[Pool(typeof(ElesisCardPool))]
public sealed class KnightlyResolve() : ElesisBlockCard(2, CardRarity.Uncommon, 16m, 6m);

[Pool(typeof(ElesisCardPool))]
public sealed class HeavyCleave() : ElesisAttackCard(2, CardRarity.Uncommon, 20m, 8m, ElesisStyle.Destruction);

[Pool(typeof(ElesisCardPool))]
public sealed class BlazingAdvance() : ElesisAttackBlockCard(1, CardRarity.Uncommon, 8m, 6m, 4m, 3m, ElesisStyle.Flame, flameGain: 2m);

[Pool(typeof(ElesisCardPool))]
public sealed class RedComet() : ElesisAttackCard(2, CardRarity.Uncommon, 24m, 6m, ElesisStyle.Flame, flameGain: 3m);

[Pool(typeof(ElesisCardPool))]
public sealed class DuelistsGuard() : ElesisBlockCard(1, CardRarity.Uncommon, 11m, 5m, ElesisStyle.Parry, chivalryGain: 1m);

[Pool(typeof(ElesisCardPool))]
public sealed class FlameWheel() : ElesisAttackCard(2, CardRarity.Uncommon, 17m, 9m, ElesisStyle.Flame, flameGain: 3m);

[Pool(typeof(ElesisCardPool))]
public sealed class RoyalAssault() : ElesisAttackCard(3, CardRarity.Rare, 32m, 10m, ElesisStyle.Destruction);

[Pool(typeof(ElesisCardPool))]
public sealed class CrimsonOath() : ElesisAttackBlockCard(2, CardRarity.Rare, 18m, 14m, 7m, 5m);

[Pool(typeof(ElesisCardPool))]
public sealed class FinalIgnition() : ElesisAttackCard(3, CardRarity.Rare, 36m, 12m, ElesisStyle.Flame, flameGain: 4m);

[Pool(typeof(ElesisCardPool))]
public sealed class UnbrokenKnight() : ElesisBlockCard(2, CardRarity.Rare, 22m, 8m);

[Pool(typeof(ElesisCardPool))]
public sealed class SwordOfBelder() : ElesisAttackCard(2, CardRarity.Rare, 26m, 10m);

[Pool(typeof(ElesisCardPool))]
public sealed class PhoenixStep() : ElesisAttackBlockCard(1, CardRarity.Rare, 11m, 11m, 5m, 5m, ElesisStyle.Flame, flameGain: 2m);

[Pool(typeof(ElesisCardPool))]
public sealed class ScarletJudgment() : ElesisAttackCard(3, CardRarity.Rare, 40m, 10m, ElesisStyle.Destruction);

[Pool(typeof(ElesisCardPool))]
public sealed class KnightCaptain() : ElesisBlockCard(1, CardRarity.Rare, 14m, 7m, ElesisStyle.Vitality, chivalryGain: 3m);

[Pool(typeof(ElesisCardPool))]
public sealed class BurningResolve() : ElesisAttackBlockCard(2, CardRarity.Rare, 16m, 16m, 6m, 6m, ElesisStyle.Flame, flameGain: 3m);

[Pool(typeof(ElesisCardPool))]
public sealed class ElLadyEcho() : ElesisAttackCard(2, CardRarity.Rare, 28m, 9m);

[Pool(typeof(ElesisCardPool))]
public sealed class GrandCrossCut() : ElesisAttackCard(3, CardRarity.Rare, 44m, 12m, ElesisStyle.Destruction);

[Pool(typeof(ElesisCardPool))]
public sealed class CrimsonFinale() : ElesisAttackBlockCard(3, CardRarity.Rare, 30m, 18m, 10m, 6m, ElesisStyle.Destruction);

[Pool(typeof(ElesisCardPool))]
public sealed class ElswordLegacy() : ElesisAttackCard(3, CardRarity.Rare, 48m, 12m, ElesisStyle.Destruction);
