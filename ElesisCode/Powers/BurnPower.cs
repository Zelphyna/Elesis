using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Elesis.ElesisCode.Extensions;

namespace Elesis.ElesisCode.Powers;

public sealed class BurnPower : ElesisPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldReceiveCombatHooks => true;
    public override string CustomPackedIconPath => "burn_power.png".PowerImagePath();
    public override string CustomBigIconPath => "burn_power.png".BigPowerImagePath();

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (Amount <= 0 || !Owner.IsMonster || Owner.Side != side || !participants.Contains(Owner))
        {
            return;
        }

        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Move, Owner);

        var burnLoss = Math.Max(1, (int)Math.Ceiling(Amount / 2m));
        if (burnLoss >= Amount)
        {
            await PowerCmd.Remove(this);
        }
        else
        {
            await PowerCmd.ModifyAmount(choiceContext, this, -burnLoss, Owner, null, true);
        }
    }
}
