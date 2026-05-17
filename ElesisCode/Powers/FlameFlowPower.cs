using Elesis.ElesisCode.Cards;
using Elesis.ElesisCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Elesis.ElesisCode.Powers;

public sealed class FlameFlowPower : ElesisPower
{
    public override string CustomPackedIconPath => "flame_power.png".PowerImagePath();
    public override string CustomBigIconPath => "flame_power.png".BigPowerImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
        {
            Flash();
            await ElesisMechanics.GainFlame(Owner, Amount, null);
        }
    }
}
