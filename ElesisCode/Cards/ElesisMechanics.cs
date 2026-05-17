using Elesis.ElesisCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Elesis.ElesisCode.Cards;

public enum ElesisStyle
{
    None,
    Vitality,
    Destruction,
    Flame,
    Parry
}

public static class ElesisMechanics
{
    public const int ChivalryThreshold = 5;
    public const int ChivalryMax = 10;

    public static async Task GainChivalry(Creature creature, decimal amount, CardModel? source)
    {
        if (amount <= 0)
        {
            return;
        }

        var current = creature.GetPowerAmount<ChivalryPower>();
        var gain = Math.Min((int)amount, ChivalryMax - current);
        if (gain > 0)
        {
            await PowerCmd.Apply<ChivalryPower>(creature, gain, creature, source, true);
        }
    }

    public static async Task GainFlame(Creature creature, decimal amount, CardModel? source)
    {
        if (amount > 0)
        {
            await PowerCmd.Apply<FlamePower>(creature, amount, creature, source, true);
        }
    }

    public static async Task<bool> TrySpendChivalry(Creature creature, int amount)
    {
        var power = creature.HasPower<ChivalryPower>() ? creature.GetPower<ChivalryPower>() : null;
        if (power is null || power.Amount < amount)
        {
            return false;
        }

        if (power.Amount == amount)
        {
            await PowerCmd.Remove(power);
        }
        else
        {
            await PowerCmd.ModifyAmount(power, -amount, creature, null, true);
        }

        return true;
    }

    public static async Task<int> ConsumeFlame(Creature creature)
    {
        var power = creature.HasPower<FlamePower>() ? creature.GetPower<FlamePower>() : null;
        if (power is null)
        {
            return 0;
        }

        var amount = power.Amount;
        await PowerCmd.Remove(power);
        return amount;
    }

    public static async Task ResolveVitalityThreshold(PlayerChoiceContext choiceContext, Player owner)
    {
        if (await TrySpendChivalry(owner.Creature, ChivalryThreshold))
        {
            await CardPileCmd.Draw(choiceContext, 1m, owner, false);
        }
    }
}
