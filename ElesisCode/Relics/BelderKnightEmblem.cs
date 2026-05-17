using Elesis.ElesisCode.Powers;
using Elesis.ElesisCode.Specializations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Relics;

public sealed class BelderKnightEmblem : ElesisRelic
{
    private const int StartingChivalry = 2;
    private int _experience;
    private int _lastProcessedNodeCount;
    private int _specialization;
    private bool _specializationChoicePending;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => Specialization == ElesisSpecialization.None;
    public override int DisplayAmount => Experience;

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int Experience
    {
        get => _experience;
        set
        {
            AssertMutable();
            _experience = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int LastProcessedNodeCount
    {
        get => _lastProcessedNodeCount;
        set
        {
            AssertMutable();
            _lastProcessedNodeCount = value;
        }
    }

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int SavedSpecialization
    {
        get => _specialization;
        set
        {
            AssertMutable();
            _specialization = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public bool SpecializationChoicePending
    {
        get => _specializationChoicePending;
        set
        {
            AssertMutable();
            _specializationChoicePending = value;
        }
    }

    public ElesisSpecialization Specialization => (ElesisSpecialization)SavedSpecialization;

    public override async Task BeforeCombatStart()
    {
        Flash();
        await PowerCmd.Apply<ChivalryPower>(Owner.Creature, StartingChivalry, Owner.Creature, null, true);
    }

    public void GainExperience(int amount)
    {
        if (Specialization != ElesisSpecialization.None || amount <= 0)
        {
            return;
        }

        Experience += amount;
        Flash();
    }

    public bool ShouldOpenSpecializationChoice(int threshold)
    {
        return Specialization == ElesisSpecialization.None
            && !SpecializationChoicePending
            && Experience >= threshold;
    }

    public void SelectSpecialization(ElesisSpecialization specialization)
    {
        if (specialization == ElesisSpecialization.None)
        {
            return;
        }

        SavedSpecialization = (int)specialization;
        SpecializationChoicePending = false;
        Status = RelicStatus.Active;
        Flash();
    }

    public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (giver != Owner.Creature)
        {
            return amount;
        }

        return Specialization switch
        {
            ElesisSpecialization.SaberKnight when power is ChivalryPower => amount + 1,
            ElesisSpecialization.PyroKnight when power is FlamePower => amount + 1,
            _ => amount
        };
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return Specialization == ElesisSpecialization.DarkKnight && dealer == Owner.Creature ? 1 : 0;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        return Specialization == ElesisSpecialization.SoarKnight && target == Owner.Creature ? 1 : 0;
    }
}
