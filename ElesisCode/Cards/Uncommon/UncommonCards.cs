using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using Elesis.ElesisCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Cards.Uncommon;

[Pool(typeof(ElesisCardPool))]
public sealed class ReprisalCut() : ElesisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new PowerVar<ReprisalCutPower>(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
        await ElesisMechanics.ApplyMarkerPower<ReprisalCutPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(ReprisalCutPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars[nameof(ReprisalCutPower)].UpgradeValueBy(3);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class BurningReversal() : ElesisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(6)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (ElesisMechanics.HasBurn(cardPlay.Target))
        {
            await ElesisMechanics.GainCounterAttack(
                choiceContext,
                Owner.Creature,
                DynamicVars[nameof(CounterAttackPower)].IntValue,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(3);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class CinderBarrage() : ElesisCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("Hits", 3),
        new PowerVar<BurnPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        for (var hit = 0; hit < DynamicVars["Hits"].IntValue; hit++)
        {
            await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
            if (!cardPlay.Target.IsAlive)
            {
                break;
            }

            await ElesisMechanics.ApplyBurn(
                choiceContext,
                cardPlay.Target,
                DynamicVars[nameof(BurnPower)].IntValue,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class PressureBreak() : ElesisCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15m, ValueProp.Move),
        new PowerVar<VulnerablePower>(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (ElesisMechanics.CounterAmount(Owner.Creature) > 0)
        {
            await ElesisMechanics.ApplyVulnerable(
                choiceContext,
                cardPlay.Target,
                DynamicVars[nameof(VulnerablePower)].IntValue,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars[nameof(VulnerablePower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class Flashpoint() : ElesisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new DynamicVar("BurnPercent", 50)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
        await ElesisMechanics.TriggerBurn(
            choiceContext,
            cardPlay.Target,
            DynamicVars["BurnPercent"].IntValue / 100m,
            false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class BackstepCleave() : ElesisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.AttackAll(choiceContext, this, DynamicVars.Damage.BaseValue);

        if (ElesisMechanics.AttackingEnemyCount(this) > 0)
        {
            await ElesisMechanics.GainCounterAttack(
                choiceContext,
                Owner.Creature,
                DynamicVars[nameof(CounterAttackPower)].IntValue,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(3);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class TemperedEdge() : ElesisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("BurnDivisor", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var divisor = Math.Max(1, DynamicVars["BurnDivisor"].IntValue);
        var bonusDamage = ElesisMechanics.BurnAmount(cardPlay.Target) / divisor;
        await ElesisMechanics.Attack(
            choiceContext,
            this,
            cardPlay.Target,
            DynamicVars.Damage.BaseValue + bonusDamage);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["BurnDivisor"].UpgradeValueBy(-1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class ProvokedAssault() : ElesisCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(16m, ValueProp.Move),
        new DynamicVar("CounterPerHit", 2),
        new DynamicVar("CounterCap", 10)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var intendedHits = ElesisMechanics.IntentHitCount(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        var counter = Math.Min(
            DynamicVars["CounterCap"].IntValue,
            intendedHits * DynamicVars["CounterPerHit"].IntValue);
        await ElesisMechanics.GainCounterAttack(choiceContext, Owner.Creature, counter, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["CounterPerHit"].UpgradeValueBy(1);
        DynamicVars["CounterCap"].UpgradeValueBy(5);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class CharredWound() : ElesisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("BurnCap", 8)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        var additionalBurn = Math.Min(
            DynamicVars["BurnCap"].IntValue,
            ElesisMechanics.BurnAmount(cardPlay.Target) / 2);
        await ElesisMechanics.ApplyBurn(
            choiceContext,
            cardPlay.Target,
            additionalBurn,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["BurnCap"].UpgradeValueBy(4);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class TwinVerdict() : ElesisCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Hits", 2),
        new PowerVar<BurnPower>(3),
        new PowerVar<CounterAttackPower>(4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
        if (!cardPlay.Target.IsAlive)
        {
            return;
        }

        await ElesisMechanics.ApplyBurn(
            choiceContext,
            cardPlay.Target,
            DynamicVars[nameof(BurnPower)].IntValue,
            Owner.Creature,
            this);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(1);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(2);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class LayeredDefense() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(4),
        new DynamicVar("PerEnemy", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attackingEnemies = ElesisMechanics.AttackingEnemyCount(this);
        var perEnemy = DynamicVars["PerEnemy"].IntValue;
        var block = DynamicVars.Block.BaseValue + attackingEnemies * perEnemy;
        var counter = DynamicVars[nameof(CounterAttackPower)].IntValue + attackingEnemies * perEnemy;

        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, block);
        await ElesisMechanics.GainCounterAttack(choiceContext, Owner.Creature, counter, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(2);
        DynamicVars["PerEnemy"].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class Countermeasure() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<CounterAttackPower>(9),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var alreadyHadCounter = ElesisMechanics.CounterAmount(Owner.Creature) > 0;
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);

        if (alreadyHadCounter)
        {
            await ElesisMechanics.Draw(choiceContext, Owner, DynamicVars.Cards.IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(4);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class ShelterInSparks() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);

        if (ElesisMechanics.Opponents(this).Any(ElesisMechanics.HasBurn))
        {
            await ElesisMechanics.GainCounterAttack(
                choiceContext,
                Owner.Creature,
                DynamicVars[nameof(CounterAttackPower)].IntValue,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(2);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class FanTheAshes() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BurnPower>(6),
        new PowerVar<BurnStabilizerPower>(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var opponent in ElesisMechanics.Opponents(this))
        {
            await ElesisMechanics.ApplyBurn(
                choiceContext,
                opponent,
                DynamicVars[nameof(BurnPower)].IntValue,
                Owner.Creature,
                this);
            await ElesisMechanics.ApplyMarkerPower<BurnStabilizerPower>(
                choiceContext,
                opponent,
                DynamicVars[nameof(BurnStabilizerPower)].IntValue,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(2);
        DynamicVars[nameof(BurnStabilizerPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class TransferHeat() : ElesisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BurnCap", 6)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var removed = await ElesisMechanics.RemoveBurn(
            choiceContext,
            cardPlay.Target,
            DynamicVars["BurnCap"].IntValue);
        await ElesisMechanics.GainCounterAttack(choiceContext, Owner.Creature, removed, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BurnCap"].UpgradeValueBy(3);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class Redirection() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new PowerVar<RedirectionPower>(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);
        await ElesisMechanics.ApplyMarkerPower<RedirectionPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(RedirectionPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars[nameof(RedirectionPower)].UpgradeValueBy(3);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class CalculatedRisk() : ElesisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("HpLoss", 3),
        new PowerVar<CounterAttackPower>(10)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.LoseHp(choiceContext, this, DynamicVars["HpLoss"].IntValue);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HpLoss"].UpgradeValueBy(-1);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(4);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class CinderScreen() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("BlockPerEnemy", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var burnedEnemies = ElesisMechanics.Opponents(this).Count(ElesisMechanics.HasBurn);
        var block = DynamicVars.Block.BaseValue +
                    burnedEnemies * DynamicVars["BlockPerEnemy"].IntValue;
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, block);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["BlockPerEnemy"].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class BurningPatience() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Retain];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BurnPower>(10)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.ApplyBurn(
            choiceContext,
            cardPlay.Target,
            DynamicVars[nameof(BurnPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(4);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class SteelNerves() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12m, ValueProp.Move),
        new PowerVar<BurnPower>(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);

        if (ElesisMechanics.AttackingEnemyCount(this) == 0)
        {
            foreach (var opponent in ElesisMechanics.Opponents(this))
            {
                await ElesisMechanics.ApplyBurn(
                    choiceContext,
                    opponent,
                    DynamicVars[nameof(BurnPower)].IntValue,
                    Owner.Creature,
                    this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(2);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class EchoGuard() : ElesisCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(13m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(8),
        new PowerVar<DoubleCounterPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);
        await ElesisMechanics.ApplyMarkerPower<DoubleCounterPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(DoubleCounterPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(2);
        DynamicVars[nameof(DoubleCounterPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class AshReclamation() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new DynamicVar("BurnPerCard", 5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var removed = await ElesisMechanics.RemoveBurn(choiceContext, cardPlay.Target);
        var burnPerCard = Math.Max(1, DynamicVars["BurnPerCard"].IntValue);
        var cards = Math.Min(DynamicVars.Cards.IntValue, removed / burnPerCard);
        await ElesisMechanics.Draw(choiceContext, Owner, cards);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars["BurnPerCard"].UpgradeValueBy(-1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class SharedThreat() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CounterAttackPower>(12)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var counter = Math.Min(
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            ElesisMechanics.BurnAmount(cardPlay.Target));
        await ElesisMechanics.GainCounterAttack(choiceContext, Owner.Creature, counter, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(6);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class WatchTheBlade() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new PowerVar<CounterAttackPower>(6)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.Draw(choiceContext, Owner, DynamicVars.Cards.IntValue);

        if (ElesisMechanics.AttackingEnemyCount(this) > 0)
        {
            await ElesisMechanics.GainCounterAttack(
                choiceContext,
                Owner.Creature,
                DynamicVars[nameof(CounterAttackPower)].IntValue,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(4);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class SmolderingGuard() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9m, ValueProp.Move),
        new PowerVar<SmolderingGuardPower>(6)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);
        await ElesisMechanics.ApplyMarkerPower<SmolderingGuardPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(SmolderingGuardPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars[nameof(SmolderingGuardPower)].UpgradeValueBy(3);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class DelayTheStrike() : ElesisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(2),
        new PowerVar<BurnPower>(7)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.ApplyWeak(
            choiceContext,
            cardPlay.Target,
            DynamicVars[nameof(WeakPower)].IntValue,
            Owner.Creature,
            this);

        if (ElesisMechanics.IntendsToAttack(cardPlay.Target))
        {
            await ElesisMechanics.ApplyBurn(
                choiceContext,
                cardPlay.Target,
                DynamicVars[nameof(BurnPower)].IntValue,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(WeakPower)].UpgradeValueBy(1);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(3);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class TacticalWithdrawal() : ElesisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new PowerVar<CounterRetentionPower>(7)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.Draw(choiceContext, Owner, DynamicVars.Cards.IntValue);
        await ElesisMechanics.ApplyMarkerPower<CounterRetentionPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterRetentionPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(CounterRetentionPower)].UpgradeValueBy(5);
    }
}

public abstract class ElesisUncommonPowerCard<TPower>(
    int cost,
    int amount,
    int upgradeAmount,
    bool usesCounterAttack,
    bool usesBurn) :
    ElesisCard(cost, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    where TPower : PowerModel, new()
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TPower>()];

    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        (usesCounterAttack ? ElesisKeywords.ForCounterAttack() : [])
        .Concat(usesBurn ? ElesisKeywords.ForBurn() : []);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TPower>(amount)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.ApplyMarkerPower<TPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars[typeof(TPower).Name].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[typeof(TPower).Name].UpgradeValueBy(upgradeAmount);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class EverReady() :
    ElesisUncommonPowerCard<EverReadyPower>(1, 2, 1, usesCounterAttack: true, usesBurn: false);

[Pool(typeof(ElesisCardPool))]
public sealed class CinderEtching() :
    ElesisUncommonPowerCard<CinderEtchingPower>(1, 2, 1, usesCounterAttack: false, usesBurn: true);

[Pool(typeof(ElesisCardPool))]
public sealed class TemperedRetort() :
    ElesisUncommonPowerCard<TemperedRetortPower>(1, 3, 2, usesCounterAttack: true, usesBurn: true);

[Pool(typeof(ElesisCardPool))]
public sealed class AegisTeeth() :
    ElesisUncommonPowerCard<AegisTeethPower>(1, 1, 1, usesCounterAttack: true, usesBurn: false);

[Pool(typeof(ElesisCardPool))]
public sealed class AshenShelter() :
    ElesisUncommonPowerCard<AshenShelterPower>(1, 3, 2, usesCounterAttack: false, usesBurn: true);

[Pool(typeof(ElesisCardPool))]
public sealed class PunishingRhythm() :
    ElesisUncommonPowerCard<PunishingRhythmPower>(1, 3, 2, usesCounterAttack: true, usesBurn: false);

[Pool(typeof(ElesisCardPool))]
public sealed class DeepScorch() :
    ElesisUncommonPowerCard<DeepScorchPower>(1, 3, 2, usesCounterAttack: false, usesBurn: true);

[Pool(typeof(ElesisCardPool))]
public sealed class FallingEmbers() :
    ElesisUncommonPowerCard<FallingEmbersPower>(1, 2, 1, usesCounterAttack: false, usesBurn: true);

[Pool(typeof(ElesisCardPool))]
public sealed class FurnaceRampart() :
    ElesisUncommonPowerCard<FurnaceRampartPower>(2, 7, 3, usesCounterAttack: true, usesBurn: false);
