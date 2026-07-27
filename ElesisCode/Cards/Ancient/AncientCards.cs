using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;
using Elesis.ElesisCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Cards.Ancient;

[Pool(typeof(ElesisCardPool))]
public sealed class RedEclipse() : ElesisCard(2, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(18m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var opponents = ElesisMechanics.Opponents(this).Where(opponent => opponent.IsAlive).ToArray();
        var command = await ElesisMechanics.AttackAll(
            choiceContext,
            this,
            DynamicVars.Damage.BaseValue);
        var damagedEnemies = command.Results
            .SelectMany(results => results)
            .Where(result => result.UnblockedDamage > 0)
            .Select(result => result.Receiver)
            .Distinct()
            .ToArray();

        foreach (var opponent in opponents.Where(opponent => opponent.IsAlive))
        {
            await ElesisMechanics.TriggerBurn(choiceContext, opponent, 1m, false);
        }

        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].BaseValue * damagedEnemies.Length,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(2m);
    }
}

[Pool(typeof(ElesisCardPool))]
public sealed class BeldersLastStand() : ElesisCard(2, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<CardKeyword> GameplayKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords =>
        ElesisKeywords.ForCounterAttack().Concat(ElesisKeywords.ForBurn());

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(24m, ValueProp.Move),
        new PowerVar<CounterAttackPower>(18),
        new DynamicVar("BurnCap", 20m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ElesisMechanics.GainBlock(choiceContext, this, cardPlay, DynamicVars.Block.BaseValue);
        await ElesisMechanics.GainCounterAttack(
            choiceContext,
            Owner.Creature,
            DynamicVars[nameof(CounterAttackPower)].IntValue,
            this);

        foreach (var opponent in ElesisMechanics.Opponents(this).Where(opponent => opponent.IsAlive).ToArray())
        {
            var addedBurn = Math.Min(
                ElesisMechanics.BurnAmount(opponent),
                DynamicVars["BurnCap"].IntValue);
            await ElesisMechanics.ApplyBurn(
                choiceContext,
                opponent,
                addedBurn,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(8m);
        DynamicVars[nameof(CounterAttackPower)].UpgradeValueBy(6m);
        DynamicVars["BurnCap"].UpgradeValueBy(10m);
    }
}
