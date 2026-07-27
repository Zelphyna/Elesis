using Elesis.ElesisCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Powers;

public abstract class ElesisBuffPower : ElesisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;
}

public abstract class ElesisDebuffPower : ElesisPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;
}

public sealed class EverReadyPower : ElesisBuffPower
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            Flash();
            await ElesisMechanics.GainCounterAttack(choiceContext, Owner, Amount, null);
        }
    }
}

public sealed class CinderEtchingPower : ElesisBuffPower
{
    private bool _usedThisTurn;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            _usedThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (_usedThisTurn || command.Attacker != Owner || command.ModelSource is not CardModel card || card.Type != CardType.Attack)
        {
            return;
        }

        var target = command.Results.SelectMany(result => result)
            .FirstOrDefault(result => result.TotalDamage > 0)?.Receiver;
        if (target is null)
        {
            return;
        }

        _usedThisTurn = true;
        Flash();
        await ElesisMechanics.ApplyBurn(choiceContext, target, Amount, Owner, card);
    }
}

public sealed class TemperedRetortPower : ElesisBuffPower
{
    private readonly HashSet<Creature> _triggeredEnemies = [];

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Enemy)
        {
            _triggeredEnemies.Clear();
        }
        return Task.CompletedTask;
    }

    public async Task OnCounterDamage(PlayerChoiceContext choiceContext, Creature enemy)
    {
        if (!_triggeredEnemies.Add(enemy))
        {
            return;
        }

        Flash();
        await ElesisMechanics.ApplyBurn(choiceContext, enemy, Amount, Owner, null);
    }
}

public sealed class AegisTeethPower : ElesisBuffPower;

public sealed class AshenShelterPower : ElesisBuffPower
{
    public async Task OnBurnDamage()
    {
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}

public sealed class PunishingRhythmPower : ElesisBuffPower
{
    private bool _usedThisEnemyTurn;

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Enemy)
        {
            _usedThisEnemyTurn = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult damageResult,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (_usedThisEnemyTurn ||
            target != Owner ||
            dealer is null ||
            dealer.Side == Owner.Side ||
            damageResult.TotalDamage <= 0 ||
            !props.IsPoweredAttack())
        {
            return;
        }

        _usedThisEnemyTurn = true;
        Flash();
        await ElesisMechanics.GainCounterAttack(choiceContext, Owner, Amount, null);
    }
}

public sealed class DeepScorchPower : ElesisBuffPower
{
    private bool _usedThisTurn;

    public bool TryUseThisTurn()
    {
        if (_usedThisTurn)
        {
            return false;
        }
        _usedThisTurn = true;
        return true;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            _usedThisTurn = false;
        }
        return Task.CompletedTask;
    }
}

public sealed class FallingEmbersPower : ElesisBuffPower
{
    public async Task OnBurnDecay(PlayerChoiceContext choiceContext, Creature decayedEnemy)
    {
        var other = ElesisMechanics.Opponents(Owner)
            .Where(enemy => enemy != decayedEnemy && enemy.IsAlive)
            .OrderBy(enemy => enemy.CombatId)
            .FirstOrDefault();

        Flash();
        if (other is not null)
        {
            await ElesisMechanics.ApplyBurn(choiceContext, other, Amount, Owner, null);
        }
        else if (decayedEnemy.IsAlive)
        {
            await ElesisMechanics.ApplyBurn(choiceContext, decayedEnemy, Math.Max(1, Amount - 1), Owner, null);
        }
    }
}

public sealed class FurnaceRampartPower : ElesisBuffPower
{
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Enemy && ElesisMechanics.CounterAmount(Owner) > 0)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        }
    }
}

public sealed class UnfadingGuardPower : ElesisBuffPower;
public sealed class BankedInfernoPower : ElesisBuffPower;
public sealed class HallOfMirrorsPower : ElesisBuffPower;

public sealed class PersistentBlazePower : ElesisBuffPower
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner)
        {
            return;
        }

        Flash();
        foreach (var enemy in ElesisMechanics.Opponents(Owner).Where(enemy => enemy.IsAlive))
        {
            await ElesisMechanics.ApplyBurn(choiceContext, enemy, Amount, Owner, null);
        }
    }
}

public sealed class AshenTriumphPower : ElesisBuffPower
{
    private int _usesThisTurn;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            _usesThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public async Task OnBurnKill(PlayerChoiceContext choiceContext)
    {
        if (_usesThisTurn >= Amount || Owner.Player is null)
        {
            return;
        }

        _usesThisTurn++;
        Flash();
        await PlayerCmd.GainEnergy(1, Owner.Player);
        await CardPileCmd.Draw(choiceContext, 2, Owner.Player);
    }
}

public sealed class HeatInTheWoundPower : ElesisBuffPower;

public sealed class PainIntoPlatePower : ElesisBuffPower
{
    private bool _usedThisEnemyTurn;

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Enemy)
        {
            _usedThisEnemyTurn = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult damageResult,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (_usedThisEnemyTurn ||
            target != Owner ||
            dealer is null ||
            dealer.Side == Owner.Side ||
            damageResult.UnblockedDamage <= 0 ||
            !props.IsPoweredAttack())
        {
            return;
        }

        _usedThisEnemyTurn = true;
        Flash();
        await CreatureCmd.GainBlock(Owner, Math.Min(Amount, damageResult.UnblockedDamage), ValueProp.Unpowered, null);
    }
}

public sealed class AfterburnPower : ElesisBuffPower
{
    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        var enemy = command.Attacker;
        if (enemy is null || enemy.Side == Owner.Side || !ElesisMechanics.HasBurn(enemy))
        {
            return;
        }

        Flash();
        await ElesisMechanics.TriggerBurn(choiceContext, enemy, Amount >= 2 ? 1m : 0.5m, false);
    }
}

public sealed class PairedResolvePower : ElesisBuffPower
{
    private CardModel? _activeCard;

    public bool TryUseFor(CardModel card)
    {
        if (_activeCard == card)
        {
            return false;
        }
        _activeCard = card;
        return true;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_activeCard == cardPlay.Card)
        {
            _activeCard = null;
        }
        return Task.CompletedTask;
    }
}

// Short-lived powers used by individual cards.
public sealed class ReprisalCutPower : ElesisBuffPower
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner && Owner.Powers.Contains(this))
        {
            await PowerCmd.Remove(this);
        }
    }
}

public sealed class CounterHistoryPower : ElesisBuffPower
{
    protected override bool IsVisibleInternal => false;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player && participants.Contains(Owner))
        {
            await PowerCmd.Remove(this);
        }
    }
}

public sealed class BurnStabilizerPower : ElesisDebuffPower;

public sealed class SealedCounterPower : ElesisBuffPower
{
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterBlockCleared(Creature creature)
    {
        if (creature == Owner &&
            Owner.Powers.Contains(this) &&
            ElesisMechanics.CounterAmount(Owner) <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}

public sealed class DoubleCounterPower : ElesisBuffPower
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner && Owner.Powers.Contains(this))
        {
            await PowerCmd.Remove(this);
        }
    }
}

public sealed class CounterRetentionPower : ElesisBuffPower
{
    public override async Task AfterBlockCleared(Creature creature)
    {
        if (creature == Owner &&
            Owner.Powers.Contains(this) &&
            ElesisMechanics.CounterAmount(Owner) <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}

public sealed class RedirectionPower : ElesisBuffPower
{
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult damageResult,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner ||
            dealer is null ||
            dealer.Side == Owner.Side ||
            damageResult.TotalDamage <= 0 ||
            !props.IsPoweredAttack())
        {
            return;
        }

        Flash();
        await ElesisMechanics.ApplyBurn(choiceContext, dealer, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }
}

public sealed class SmolderingGuardPower : ElesisBuffPower
{
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult damageResult,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner ||
            dealer is null ||
            dealer.Side == Owner.Side ||
            !damageResult.WasBlockBroken ||
            !props.IsPoweredAttack())
        {
            return;
        }

        Flash();
        await ElesisMechanics.ApplyBurn(choiceContext, dealer, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }
}

public sealed class AggressorBrandPower : ElesisDebuffPower
{
    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker != Owner ||
            Applier is null ||
            !command.DamageProps.IsPoweredAttack())
        {
            return;
        }

        var hitCount = command.Results
            .SelectMany(results => results)
            .Count(result => result.TotalDamage > 0 && result.Receiver.Side != Owner.Side);

        for (var hit = 0; hit < hitCount && Owner.IsAlive; hit++)
        {
            Flash();
            await ElesisMechanics.ApplyBurn(choiceContext, Owner, Amount, Applier, null);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
