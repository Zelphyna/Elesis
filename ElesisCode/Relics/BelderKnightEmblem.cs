using Elesis.ElesisCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Elesis.ElesisCode.Relics;

public sealed class BelderKnightEmblem : ElesisRelic
{
    private const int StartingChivalry = 2;

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task BeforeCombatStart()
    {
        Flash();
        await PowerCmd.Apply<ChivalryPower>(Owner.Creature, StartingChivalry, Owner.Creature, null, true);
    }
}
