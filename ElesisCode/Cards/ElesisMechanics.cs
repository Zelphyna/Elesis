using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using Elesis.ElesisCode.Powers;

namespace Elesis.ElesisCode.Cards;

public static class ElesisMechanics
{
    public static IReadOnlyList<Creature> Opponents(CardModel card) =>
        card.Owner.Creature.CombatState?.GetOpponentsOf(card.Owner.Creature) ?? [];

    public static IReadOnlyList<Creature> Opponents(Creature creature) =>
        creature.CombatState?.GetOpponentsOf(creature) ?? [];

    public static bool HasBurn(Creature creature) => BurnAmount(creature) > 0;

    public static int BurnAmount(Creature creature) =>
        creature.Powers.OfType<BurnPower>().FirstOrDefault()?.Amount ?? 0;

    public static int CounterAmount(Creature creature) =>
        creature.Powers.OfType<CounterAttackPower>().FirstOrDefault()?.Amount ?? 0;

    public static bool IntendsToAttack(Creature creature) =>
        creature.Monster?.NextMove.Intents.OfType<AttackIntent>().Any() == true;

    public static int IntentHitCount(Creature creature) =>
        creature.Monster?.NextMove.Intents.OfType<AttackIntent>().Sum(intent => Math.Max(1, intent.Repeats)) ?? 0;

    public static int AttackingEnemyCount(CardModel card) => Opponents(card).Count(IntendsToAttack);

    public static async Task<AttackCommand> Attack(
        PlayerChoiceContext choiceContext,
        CardModel card,
        Creature target,
        decimal damage,
        int hits = 1)
    {
        return await DamageCmd.Attack(damage)
            .FromCard(card)
            .Targeting(target)
            .WithHitCount(Math.Max(1, hits))
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public static async Task<AttackCommand> AttackAll(
        PlayerChoiceContext choiceContext,
        CardModel card,
        decimal damage,
        int hits = 1)
    {
        ArgumentNullException.ThrowIfNull(card.Owner.Creature.CombatState);
        return await DamageCmd.Attack(damage)
            .FromCard(card)
            .TargetingAllOpponents(card.Owner.Creature.CombatState)
            .WithHitCount(Math.Max(1, hits))
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public static async Task GainBlock(
        PlayerChoiceContext choiceContext,
        CardModel card,
        CardPlay cardPlay,
        decimal amount)
    {
        if (amount <= 0)
        {
            return;
        }

        var gained = await CreatureCmd.GainBlock(card.Owner.Creature, amount, ValueProp.Move, cardPlay);
        var aegis = card.Owner.Creature.Powers.OfType<AegisTeethPower>().FirstOrDefault();
        if (gained > 0 && aegis is not null)
        {
            aegis.TriggerFlash();
            await GainCounterAttack(choiceContext, card.Owner.Creature, aegis.Amount, card);
        }
    }

    public static Task Draw(PlayerChoiceContext choiceContext, Player player, int cards) =>
        cards > 0 ? CardPileCmd.Draw(choiceContext, cards, player) : Task.CompletedTask;

    public static Task ApplyWeak(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature source,
        CardModel? cardSource) =>
        amount > 0
            ? PowerCmd.Apply<WeakPower>(choiceContext, target, amount, source, cardSource, true)
            : Task.CompletedTask;

    public static Task ApplyVulnerable(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature source,
        CardModel? cardSource) =>
        amount > 0
            ? PowerCmd.Apply<VulnerablePower>(choiceContext, target, amount, source, cardSource, true)
            : Task.CompletedTask;

    public static async Task GainCounterAttack(PlayerChoiceContext choiceContext, Creature creature, decimal amount, CardModel? source)
    {
        if (amount > 0)
        {
            await PowerCmd.Apply<CounterAttackPower>(choiceContext, creature, amount, creature, source, true);
        }
    }

    public static async Task ApplyBurn(PlayerChoiceContext choiceContext, Creature creature, decimal amount, Creature source, CardModel? cardSource)
    {
        if (amount <= 0 || !creature.IsAlive)
        {
            return;
        }

        var deepScorch = source.Powers.OfType<DeepScorchPower>().FirstOrDefault();
        if (deepScorch is not null && deepScorch.TryUseThisTurn())
        {
            deepScorch.TriggerFlash();
            amount += deepScorch.Amount;
        }

        await PowerCmd.Apply<BurnPower>(choiceContext, creature, amount, source, cardSource, true);

        if (cardSource is not null)
        {
            var pairedResolve = source.Powers.OfType<PairedResolvePower>().FirstOrDefault();
            if (pairedResolve is not null && pairedResolve.TryUseFor(cardSource))
            {
                pairedResolve.TriggerFlash();
                await GainCounterAttack(choiceContext, source, pairedResolve.Amount, cardSource);
            }
        }
    }

    public static async Task<int> RemoveBurn(PlayerChoiceContext choiceContext, Creature creature, decimal maximum = decimal.MaxValue)
    {
        var burn = creature.Powers.OfType<BurnPower>().FirstOrDefault();
        if (burn is null || maximum <= 0)
        {
            return 0;
        }

        var removed = Math.Min(burn.Amount, (int)Math.Floor(maximum));
        if (removed >= burn.Amount)
        {
            await PowerCmd.Remove(burn);
        }
        else
        {
            await PowerCmd.ModifyAmount(choiceContext, burn, -removed, burn.Applier, null, true);
        }

        return removed;
    }

    public static async Task<int> ReduceBurnByFraction(
        PlayerChoiceContext choiceContext,
        Creature creature,
        decimal fraction)
    {
        var amount = BurnAmount(creature);
        if (amount <= 0)
        {
            return 0;
        }

        return await RemoveBurn(choiceContext, creature, Math.Ceiling(amount * fraction));
    }

    public static async Task<int> TriggerBurn(
        PlayerChoiceContext choiceContext,
        Creature creature,
        decimal fraction = 1m,
        bool decay = false)
    {
        var burn = creature.Powers.OfType<BurnPower>().FirstOrDefault();
        if (burn is null || burn.Amount <= 0 || fraction <= 0)
        {
            return 0;
        }

        return await burn.Trigger(choiceContext, fraction, decay);
    }

    public static async Task LoseHp(
        PlayerChoiceContext choiceContext,
        CardModel card,
        decimal amount)
    {
        if (amount > 0)
        {
            await CreatureCmd.Damage(
                choiceContext,
                card.Owner.Creature,
                amount,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                card.Owner.Creature,
                card);
        }
    }

    public static Task ApplyMarkerPower<T>(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature source,
        CardModel? cardSource)
        where T : PowerModel, new() =>
        amount > 0
            ? PowerCmd.Apply<T>(choiceContext, target, amount, source, cardSource, true)
            : Task.CompletedTask;

    public static async Task ApplyMinimumPower<T>(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature source,
        CardModel? cardSource)
        where T : PowerModel, new()
    {
        if (amount <= 0)
        {
            return;
        }

        var existing = target.Powers.OfType<T>().FirstOrDefault();
        if (existing is null)
        {
            await PowerCmd.Apply<T>(choiceContext, target, amount, source, cardSource, true);
            return;
        }

        var desired = (int)Math.Floor(amount);
        if (desired < existing.Amount)
        {
            await PowerCmd.ModifyAmount(
                choiceContext,
                existing,
                desired - existing.Amount,
                source,
                cardSource,
                true);
        }
    }

    public static async Task SetCounterAmount(
        PlayerChoiceContext choiceContext,
        Creature creature,
        int amount,
        CardModel? source = null)
    {
        var counter = creature.Powers.OfType<CounterAttackPower>().FirstOrDefault();
        if (amount <= 0)
        {
            if (counter is not null)
            {
                await PowerCmd.Remove(counter);
            }
            return;
        }

        if (counter is null)
        {
            await GainCounterAttack(choiceContext, creature, amount, source);
            return;
        }

        var delta = amount - counter.Amount;
        if (delta != 0)
        {
            await PowerCmd.ModifyAmount(choiceContext, counter, delta, creature, source, true);
        }
    }
}
