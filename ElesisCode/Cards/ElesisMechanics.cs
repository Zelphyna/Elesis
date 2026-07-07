using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Elesis.ElesisCode.Powers;

namespace Elesis.ElesisCode.Cards;

public static class ElesisMechanics
{
    public static async Task GainCounterAttack(PlayerChoiceContext choiceContext, Creature creature, decimal amount, CardModel? source)
    {
        if (amount > 0)
        {
            await PowerCmd.Apply<CounterAttackPower>(choiceContext, creature, amount, creature, source, true);
        }
    }

    public static async Task ApplyBurn(PlayerChoiceContext choiceContext, Creature creature, decimal amount, Creature source, CardModel? cardSource)
    {
        if (amount > 0)
        {
            await PowerCmd.Apply<BurnPower>(choiceContext, creature, amount, source, cardSource, true);
        }
    }
}
