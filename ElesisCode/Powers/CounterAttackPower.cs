using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
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
        if (Amount > 0 && creature == Owner)
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
        if (Amount <= 0 || target != Owner || damageResult.TotalDamage <= 0 || dealer is null)
        {
            return;
        }

        if (dealer == Owner || dealer.Side == Owner.Side)
        {
            return;
        }

        Flash();
        await CreatureCmd.Damage(choiceContext, dealer, Amount, ValueProp.Move, Owner);
    }
}
