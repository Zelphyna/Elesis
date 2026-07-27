using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using Elesis.ElesisCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Cards.Rare;

[Pool(typeof(ElesisCardPool))]
public sealed class CrimsonReprisal() : ElesisCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new DynamicVar("RepeatCount", 1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var repeatCount = Owner.Creature.Powers.OfType<CounterHistoryPower>().Any()
            ? DynamicVars["RepeatCount"].IntValue
            : 0;

        await ElesisMechanics.Attack(
            choiceContext,
            this,
            cardPlay.Target,
            DynamicVars.Damage.BaseValue,
            1 + repeatCount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["RepeatCount"].UpgradeValueBy(1m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class PyreDivide() : ElesisCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
        if (cardPlay.Target.IsAlive)
        {
            await ElesisMechanics.TriggerBurn(choiceContext, cardPlay.Target, 1m, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class PerfectAnswer() : ElesisCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20m, ValueProp.Move),
        new DynamicVar("CounterCap", 12m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var command = await ElesisMechanics.Attack(
            choiceContext,
            this,
            cardPlay.Target,
            DynamicVars.Damage.BaseValue);
        var unblockedDamage = command.Results
            .SelectMany(results => results)
            .Sum(result => result.UnblockedDamage);
        var counter = Math.Min(
            DynamicVars["CounterCap"].IntValue,
            (int)Math.Floor(unblockedDamage / 2m));

        await ElesisMechanics.GainCounterAttack(choiceContext, Owner.Creature, counter, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["CounterCap"].UpgradeValueBy(6m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class ScarletCrossfire() : ElesisCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Hits", 3m),
        new PowerVar<BurnPower>(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        for (var hit = 0; hit < DynamicVars["Hits"].IntValue && cardPlay.Target.IsAlive; hit++)
        {
            await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
            if (cardPlay.Target.IsAlive)
            {
                await ElesisMechanics.ApplyBurn(
                    choiceContext,
                    cardPlay.Target,
                    DynamicVars[nameof(BurnPower)].IntValue,
                    Owner.Creature,
                    this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(1m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class SentenceOfAsh() : ElesisCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13m, ValueProp.Move),
        new DynamicVar("BurnCap", 18m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var opponent in ElesisMechanics.Opponents(this).Where(opponent => opponent.IsAlive).ToArray())
        {
            var bonusDamage = Math.Min(
                ElesisMechanics.BurnAmount(opponent),
                DynamicVars["BurnCap"].IntValue);
            await ElesisMechanics.Attack(
                choiceContext,
                this,
                opponent,
                DynamicVars.Damage.BaseValue + bonusDamage);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["BurnCap"].UpgradeValueBy(9m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class MirrorsteelLunge() : ElesisCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<DoubleCounterPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
        await ElesisMechanics.ApplyMarkerPower<DoubleCounterPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(DoubleCounterPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars[nameof(DoubleCounterPower)].UpgradeValueBy(1m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class LastingScar() : ElesisCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("BurnCap", 20m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
        if (!cardPlay.Target.IsAlive)
        {
            return;
        }

        var addedBurn = Math.Min(
            ElesisMechanics.BurnAmount(cardPlay.Target),
            DynamicVars["BurnCap"].IntValue);
        await ElesisMechanics.ApplyBurn(
            choiceContext,
            cardPlay.Target,
            addedBurn,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["BurnCap"].UpgradeValueBy(10m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class RedHorizon() : ElesisCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(26m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(12),
        new PowerVar<BurnPower>(8)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var opponents = ElesisMechanics.Opponents(this).ToArray();
        await ElesisMechanics.AttackAll(choiceContext, this, DynamicVars.Damage.BaseValue);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);

        foreach (var opponent in opponents.Where(opponent => opponent.IsAlive))
        {
            await ElesisMechanics.ApplyBurn(
                choiceContext,
                opponent,
                DynamicVars[nameof(BurnPower)].IntValue,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(5m);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(4m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class AbsoluteGuard() : ElesisCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(22m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(15)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(8m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(6m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class SealedDefense() : ElesisCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> GameplayKeywords =>
        IsUpgraded ? [] : [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(13m, ValueProp.Move),
        new PowerVar<SealedCounterPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);
        await ElesisMechanics.ApplyMarkerPower<SealedCounterPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(SealedCounterPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
        RemoveKeyword(CardKeyword.Exhaust);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class AshCascade() : ElesisCard(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BurnPower>(14),
        new DynamicVar("TriggerFraction", 0.5m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var opponents = ElesisMechanics.Opponents(this).Where(opponent => opponent.IsAlive).ToArray();
        foreach (var opponent in opponents)
        {
            await ElesisMechanics.ApplyBurn(
                choiceContext,
                opponent,
                DynamicVars[nameof(BurnPower)].IntValue,
                Owner.Creature,
                this);
        }

        foreach (var opponent in opponents.Where(opponent => opponent.IsAlive))
        {
            await ElesisMechanics.TriggerBurn(
                choiceContext,
                opponent,
                DynamicVars["TriggerFraction"].BaseValue,
                false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(4m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class BorrowedHeat() : ElesisCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("BlockCap", 22m),
        new DynamicVar("BurnReductionDivisor", 2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var block = Math.Min(
            ElesisMechanics.BurnAmount(cardPlay.Target),
            DynamicVars["BlockCap"].IntValue);
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, block);
        await ElesisMechanics.ReduceBurnByFraction(
            choiceContext,
            cardPlay.Target,
            1m / DynamicVars["BurnReductionDivisor"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockCap"].UpgradeValueBy(10m);
        DynamicVars["BurnReductionDivisor"].UpgradeValueBy(1m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class ReturnToSender() : ElesisCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(4),
        new DynamicVar("HitCap", 5m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var intendedHits = Math.Min(
            DynamicVars["HitCap"].IntValue,
            ElesisMechanics.IntentHitCount(cardPlay.Target));

        await ElesisMechanics.GainBlock(
            choiceContext,
            this,
            cardPlay,
            DynamicVars.Block.BaseValue * intendedHits);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].BaseValue * intendedHits,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(1m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class BrandTheAggressor() : ElesisCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BurnPower>(11),
        new PowerVar<AggressorBrandPower>(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.ApplyBurn(
            choiceContext,
            cardPlay.Target,
            DynamicVars[nameof(BurnPower)].IntValue,
            Owner.Creature,
            this);
        await ElesisMechanics.ApplyMarkerPower<AggressorBrandPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars[nameof(AggressorBrandPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(4m);
        DynamicVars[nameof(AggressorBrandPower)].UpgradeValueBy(2m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class EncircledBulwark() : ElesisCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(8),
        new DynamicVar("AdditionalEnemyBonus", 5m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var additionalEnemies = Math.Max(0, ElesisMechanics.AttackingEnemyCount(this) - 1);
        var bonus = DynamicVars["AdditionalEnemyBonus"].BaseValue * additionalEnemies;
        await ElesisMechanics.GainBlock(
            choiceContext,
            this,
            cardPlay,
            DynamicVars.Block.BaseValue + bonus);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].BaseValue + bonus,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(3m);
        DynamicVars["AdditionalEnemyBonus"].UpgradeValueBy(2m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class RekindledDefense() : ElesisCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("BlockPerBurn", 2m),
        new DynamicVar("BlockCap", 35m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var removedBurn = 0;
        foreach (var opponent in ElesisMechanics.Opponents(this).ToArray())
        {
            removedBurn += await ElesisMechanics.RemoveBurn(choiceContext, opponent);
        }

        var block = Math.Min(
            DynamicVars["BlockCap"].IntValue,
            removedBurn * DynamicVars["BlockPerBurn"].IntValue);
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, block);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockPerBurn"].UpgradeValueBy(1m);
        DynamicVars["BlockCap"].UpgradeValueBy(15m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class ReadEveryBlade() : ElesisCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<CardKeyword> GameplayKeywords =>
        IsUpgraded ? [] : [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new PowerVar<CounterAttackPower>(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.Draw(choiceContext, Owner, DynamicVars.Cards.IntValue);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].BaseValue *
            ElesisMechanics.AttackingEnemyCount(this),
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(1m);
        RemoveKeyword(CardKeyword.Exhaust);
    }
}

public abstract class RarePowerCard<TPower>(
    int cost,
    decimal amount,
    decimal upgradeDelta) :
    ElesisCard(cost, CardType.Power, CardRarity.Rare, TargetType.Self)
    where TPower : PowerModel, new()
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TPower>()];

    protected string PowerVarName => typeof(TPower).Name;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TPower>(amount)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.ApplyMarkerPower<TPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[PowerVarName].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[PowerVarName].UpgradeValueBy(upgradeDelta);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class UnfadingGuard() : RarePowerCard<UnfadingGuardPower>(2, 4m, -2m)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.ApplyMinimumPower<UnfadingGuardPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(UnfadingGuardPower)].BaseValue,
            Owner.Creature,
            this);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class BankedInferno() : RarePowerCard<BankedInfernoPower>(2, 3m, 1m)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();
}

[Pool(typeof(ElesisCardPool))]
public sealed class HallOfMirrors() : RarePowerCard<HallOfMirrorsPower>(2, 1m, 1m)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<HallOfMirrorsPower>(1),
        new DynamicVar("Fraction", 0.5m)
    ];

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Fraction"].UpgradeValueBy(0.5m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class PersistentBlaze() : RarePowerCard<PersistentBlazePower>(2, 3m, 2m)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();
}

[Pool(typeof(ElesisCardPool))]
public sealed class AshenTriumph() : RarePowerCard<AshenTriumphPower>(1, 1m, 1m)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();
}

[Pool(typeof(ElesisCardPool))]
public sealed class HeatInTheWound() : RarePowerCard<HeatInTheWoundPower>(2, 4m, -1m)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.ApplyMinimumPower<HeatInTheWoundPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(HeatInTheWoundPower)].BaseValue,
            Owner.Creature,
            this);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class PainIntoPlate() : RarePowerCard<PainIntoPlatePower>(2, 18m, 12m);

[Pool(typeof(ElesisCardPool))]
public sealed class Afterburn() : RarePowerCard<AfterburnPower>(3, 1m, 1m)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<AfterburnPower>(1),
        new DynamicVar("Fraction", 0.5m)
    ];

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        DynamicVars["Fraction"].UpgradeValueBy(0.5m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class PairedResolve() : RarePowerCard<PairedResolvePower>(2, 2m, 1m)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());
}
