using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Elesis.ElesisCode.Cards;
using Elesis.ElesisCode.Extensions;

namespace Elesis.ElesisCode.Powers;

public sealed class CounterAttackPower : ElesisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;
    public override string CustomPackedIconPath => "counter_attack_power.png".PowerImagePath();
    public override string CustomBigIconPath => "counter_attack_power.png".BigPowerImagePath();

    public override async Task AfterBlockCleared(Creature creature)
    {
        if (Amount <= 0 || creature != Owner)
        {
            return;
        }

        var sealedCounter = Owner.Powers.OfType<SealedCounterPower>().FirstOrDefault();
        if (sealedCounter is not null)
        {
            await PowerCmd.Remove(sealedCounter);
            var sealedRetention = Owner.Powers.OfType<CounterRetentionPower>().FirstOrDefault();
            if (sealedRetention is not null)
            {
                await PowerCmd.Remove(sealedRetention);
            }

            return;
        }

        var retention = Owner.Powers.OfType<CounterRetentionPower>().FirstOrDefault();
        if (retention is not null)
        {
            var retained = Math.Min(Amount, retention.Amount);
            await PowerCmd.Remove(retention);
            if (retained <= 0)
            {
                await PowerCmd.Remove(this);
            }
            else if (retained != Amount)
            {
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, retained - Amount, Owner, null, true);
            }
            return;
        }

        var unfading = Owner.Powers.OfType<UnfadingGuardPower>().FirstOrDefault();
        if (unfading is not null)
        {
            var remaining = Math.Max(0, Amount - unfading.Amount);
            if (remaining == 0)
            {
                await PowerCmd.Remove(this);
            }
            else
            {
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, remaining - Amount, Owner, null, true);
            }
        }
        else
        {
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult damageResult,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Amount <= 0 ||
            target != Owner ||
            damageResult.TotalDamage <= 0 ||
            dealer is null ||
            !props.IsPoweredAttack())
        {
            return;
        }

        if (dealer == Owner || dealer.Side == Owner.Side)
        {
            return;
        }

        var counterDamage = Amount;
        var reprisal = Owner.Powers.OfType<ReprisalCutPower>().FirstOrDefault();
        if (reprisal is not null)
        {
            counterDamage += reprisal.Amount;
            await PowerCmd.Remove(reprisal);
        }

        var heat = Owner.Powers.OfType<HeatInTheWoundPower>().FirstOrDefault();
        if (heat is not null)
        {
            counterDamage += ElesisMechanics.BurnAmount(dealer) / Math.Max(1, heat.Amount);
        }

        var repeats = 1;
        var doubled = Owner.Powers.OfType<DoubleCounterPower>().FirstOrDefault();
        if (doubled is not null)
        {
            repeats = 2;
            if (doubled.Amount <= 1)
            {
                await PowerCmd.Remove(doubled);
            }
            else
            {
                await PowerCmd.ModifyAmount(choiceContext, doubled, -1, Owner, null, true);
            }
        }

        Flash();
        var dealtAnyDamage = false;
        HashSet<Creature> counterDamagedEnemies = [];
        for (var repeat = 0; repeat < repeats && dealer.IsAlive; repeat++)
        {
            var results = await CreatureCmd.Damage(choiceContext, dealer, counterDamage, ValueProp.Unpowered, Owner);
            var dealt = results.Sum(result => result.UnblockedDamage);
            if (dealt <= 0)
            {
                continue;
            }

            dealtAnyDamage = true;
            counterDamagedEnemies.Add(dealer);
            var mirrors = Owner.Powers.OfType<HallOfMirrorsPower>().FirstOrDefault();
            if (mirrors is not null)
            {
                mirrors.TriggerFlash();
                var splash = mirrors.Amount >= 2 ? dealt : Math.Floor(dealt / 2m);
                foreach (var other in ElesisMechanics.Opponents(Owner).Where(enemy => enemy != dealer && enemy.IsAlive))
                {
                    var splashResults = await CreatureCmd.Damage(
                        choiceContext,
                        other,
                        splash,
                        ValueProp.Unpowered,
                        Owner);
                    if (splashResults.Sum(result => result.UnblockedDamage) > 0)
                    {
                        counterDamagedEnemies.Add(other);
                    }
                }
            }
        }

        if (dealtAnyDamage)
        {
            if (!Owner.Powers.OfType<CounterHistoryPower>().Any())
            {
                await PowerCmd.Apply<CounterHistoryPower>(
                    choiceContext,
                    Owner,
                    1,
                    Owner,
                    null,
                    true);
            }

            var retort = Owner.Powers.OfType<TemperedRetortPower>().FirstOrDefault();
            if (retort is not null)
            {
                foreach (var enemy in counterDamagedEnemies.Where(enemy => enemy.IsAlive))
                {
                    await retort.OnCounterDamage(choiceContext, enemy);
                }
            }
        }
    }
}
