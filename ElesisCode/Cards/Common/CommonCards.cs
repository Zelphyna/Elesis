using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using Elesis.ElesisCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Cards.Common;

[Pool(typeof(ElesisCardPool))]
public sealed class MeasuredSlash() : ElesisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (ElesisMechanics.IntendsToAttack(cardPlay.Target))
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
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class EmberThrust() : ElesisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<BurnPower>(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);
        await ElesisMechanics.ApplyBurn(
            choiceContext,
            cardPlay.Target,
            DynamicVars[nameof(BurnPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class ScorchingFeint() : ElesisCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new BlockVar(3m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (ElesisMechanics.HasBurn(cardPlay.Target))
        {
            await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(1m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class GuardedCut() : ElesisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new BlockVar(4m, ValueProp.Move),
        new DynamicVar("BonusBlock", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        var block = DynamicVars.Block.BaseValue;
        if (ElesisMechanics.CounterAmount(Owner.Creature) > 0)
        {
            block += DynamicVars["BonusBlock"].IntValue;
        }

        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, block);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars["BonusBlock"].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class KindledSweep() : ElesisCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new PowerVar<BurnPower>(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.AttackAll(choiceContext, this, DynamicVars.Damage.BaseValue);

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

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class AnsweringBlow() : ElesisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (ElesisMechanics.CounterAmount(Owner.Creature) > 0)
        {
            await ElesisMechanics.Draw(choiceContext, Owner, DynamicVars.Cards.IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class Coalbrand() : ElesisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new PowerVar<BurnPower>(4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (!ElesisMechanics.HasBurn(cardPlay.Target))
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
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class OpeningRead() : ElesisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(3),
        new PowerVar<BurnPower>(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (ElesisMechanics.IntendsToAttack(cardPlay.Target))
        {
            await ElesisMechanics.GainCounterAttack(
                choiceContext,
                Owner.Creature,
                DynamicVars[nameof(CounterAttackPower)].IntValue,
                this);
        }
        else
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
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(1);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class SearingPommel() : ElesisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (ElesisMechanics.HasBurn(cardPlay.Target))
        {
            await ElesisMechanics.Draw(choiceContext, Owner, DynamicVars.Cards.IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class CrossfireSlash() : ElesisCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<BurnPower>(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ElesisMechanics.Attack(choiceContext, this, cardPlay.Target, DynamicVars.Damage.BaseValue, 2);
        await ElesisMechanics.ApplyBurn(
            choiceContext,
            cardPlay.Target,
            DynamicVars[nameof(BurnPower)].IntValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class ChallengeSweep() : ElesisCard(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(11m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.AttackAll(choiceContext, this, DynamicVars.Damage.BaseValue);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(2);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class ReadyGuard() : ElesisCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(3)
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
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class HeatHaze() : ElesisCard(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BurnPower>(4)];

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
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(2);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class LowGuard() : ElesisCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);

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
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class AshenVeil() : ElesisCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
        new DynamicVar("BonusBlock", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = DynamicVars.Block.BaseValue;
        if (ElesisMechanics.Opponents(this).Any(ElesisMechanics.HasBurn))
        {
            block += DynamicVars["BonusBlock"].IntValue;
        }

        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, block);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["BonusBlock"].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class RedChallenge() : ElesisCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1),
        new PowerVar<CounterAttackPower>(4)
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
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(WeakPower)].UpgradeValueBy(1);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class BankedSpark() : ElesisCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BurnPower>(7)];

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
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(3);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class SteadyBreath() : ElesisCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForCounterAttack();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new PowerVar<CounterAttackPower>(4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.Draw(choiceContext, Owner, DynamicVars.Cards.IntValue);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class CoveringEmbers() : ElesisCard(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> ElesisCardKeywords => ElesisKeywords.ForBurn();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new PowerVar<BurnPower>(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);

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

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars[nameof(BurnPower)].UpgradeValueBy(1);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class RootedDefense() : ElesisCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
