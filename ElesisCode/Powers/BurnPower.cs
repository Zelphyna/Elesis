using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Elesis.ElesisCode.Cards;

namespace Elesis.ElesisCode.Powers;

public sealed class BurnPower : ElesisPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (Amount <= 0 || !Owner.IsMonster || Owner.Side != side || !participants.Contains(Owner))
        {
            return;
        }

        await Trigger(new ThrowingPlayerChoiceContext(), 1m, true);
    }

    public async Task<int> Trigger(PlayerChoiceContext choiceContext, decimal fraction = 1m, bool decay = false)
    {
        if (Amount <= 0 || fraction <= 0 || !Owner.IsAlive)
        {
            return 0;
        }

        var source = Applier;
        var damage = Math.Max(0, (int)Math.Floor(Amount * fraction));
        if (damage <= 0)
        {
            return 0;
        }

        Flash();
        var wasAlive = Owner.IsAlive;
        var results = await CreatureCmd.Damage(
            choiceContext,
            Owner,
            damage,
            ValueProp.Unblockable | ValueProp.Unpowered,
            source,
            null);
        var dealt = (int)results.Sum(result => result.UnblockedDamage);

        if (dealt > 0 && source is not null)
        {
            var shelter = source.Powers.OfType<AshenShelterPower>().FirstOrDefault();
            if (shelter is not null)
            {
                await shelter.OnBurnDamage();
            }
        }

        if (wasAlive && !Owner.IsAlive && source is not null)
        {
            var triumph = source.Powers.OfType<AshenTriumphPower>().FirstOrDefault();
            if (triumph is not null)
            {
                await triumph.OnBurnKill(choiceContext);
            }
        }

        if (!decay || !Owner.IsAlive)
        {
            return dealt;
        }

        var divisor = Math.Max(2, source?.Powers.OfType<BankedInfernoPower>().FirstOrDefault()?.Amount ?? 2);
        var burnLoss = Math.Max(1, (int)Math.Ceiling(Amount / (decimal)divisor));
        var stabilizer = Owner.Powers.OfType<BurnStabilizerPower>().FirstOrDefault();
        if (stabilizer is not null)
        {
            burnLoss = Math.Max(1, burnLoss - stabilizer.Amount);
            await PowerCmd.Remove(stabilizer);
        }

        if (burnLoss >= Amount)
        {
            await PowerCmd.Remove(this);
        }
        else
        {
            await PowerCmd.ModifyAmount(choiceContext, this, -burnLoss, source, null, true);
        }

        var embers = source?.Powers.OfType<FallingEmbersPower>().FirstOrDefault();
        if (embers is not null)
        {
            await embers.OnBurnDecay(choiceContext, Owner);
        }

        return dealt;
    }
}
