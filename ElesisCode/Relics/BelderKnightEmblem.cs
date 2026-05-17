using Elesis.ElesisCode.Powers;
using Elesis.ElesisCode.Rewards;
using Elesis.ElesisCode.Specializations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Relics;

public sealed class BelderKnightEmblem : ElesisRelic
{
    private const int StartingChivalry = 2;
    private int _experience;
    private bool _combatExperienceClaimedAwaitingMap;
    private int _evolutionTier;
    private int _lastExperienceAwardedNodeCount;
    private int _lastProcessedNodeCount;
    private int _pendingEvolutionTier;
    private int _specialization;
    private bool _specializationChoicePending;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
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
    public bool CombatExperienceClaimedAwaitingMap
    {
        get => _combatExperienceClaimedAwaitingMap;
        set
        {
            AssertMutable();
            _combatExperienceClaimedAwaitingMap = value;
        }
    }

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int EvolutionTier
    {
        get => _evolutionTier;
        set
        {
            AssertMutable();
            _evolutionTier = value;
        }
    }

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int LastExperienceAwardedNodeCount
    {
        get => _lastExperienceAwardedNodeCount;
        set
        {
            AssertMutable();
            _lastExperienceAwardedNodeCount = value;
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
    public int PendingEvolutionTier
    {
        get => _pendingEvolutionTier;
        set
        {
            AssertMutable();
            _pendingEvolutionTier = value;
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
    public int SpecializationBonus => Specialization == ElesisSpecialization.None ? 0 : Math.Max(1, EvolutionTier);

    public override async Task BeforeCombatStart()
    {
        Flash();
        await PowerCmd.Apply<ChivalryPower>(Owner.Creature, StartingChivalry, Owner.Creature, null, true);
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is MapRoom)
        {
            await ElesisSpecializationController.ProcessCurrentMapEntry(this);
        }
    }

    public override async Task AfterRewardTaken(Player player, Reward reward)
    {
        if (player == Owner && reward is ElesisExperienceReward)
        {
            await ElesisSpecializationController.TryOpenPendingProgressionEvent(this, requireMapRoom: false);
        }
    }

    public void GainExperience(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Experience += amount;
        MainFile.Logger.Info($"Elesis gained XP. amount={amount} totalXp={Experience} specialization={Specialization} tier={EvolutionTier}");
        Flash();
    }

    public void ClaimCombatExperienceReward(int amount)
    {
        MainFile.Logger.Info($"Elesis XP reward selected. amount={amount} previousXp={Experience}");
        GainExperience(amount);
        CombatExperienceClaimedAwaitingMap = true;
    }

    public bool ShouldOpenSpecializationChoice(int threshold)
    {
        return Specialization == ElesisSpecialization.None
            && Experience >= threshold;
    }

    public bool ShouldOpenEvolution(int threshold, int targetTier)
    {
        return Specialization != ElesisSpecialization.None
            && EvolutionTier < targetTier
            && (PendingEvolutionTier == 0 || PendingEvolutionTier == targetTier)
            && Experience >= threshold;
    }

    public void SelectSpecialization(ElesisSpecialization specialization)
    {
        if (specialization == ElesisSpecialization.None)
        {
            return;
        }

        SavedSpecialization = (int)specialization;
        EvolutionTier = Math.Max(EvolutionTier, 1);
        SpecializationChoicePending = false;
        Status = RelicStatus.Active;
        Flash();
    }

    public void UnlockEvolution(int targetTier)
    {
        if (Specialization == ElesisSpecialization.None || targetTier <= EvolutionTier)
        {
            PendingEvolutionTier = 0;
            return;
        }

        EvolutionTier = targetTier;
        PendingEvolutionTier = 0;
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
            ElesisSpecialization.SaberKnight when power is ChivalryPower => amount + SpecializationBonus,
            ElesisSpecialization.PyroKnight when power is FlamePower => amount + SpecializationBonus,
            _ => amount
        };
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return Specialization == ElesisSpecialization.DarkKnight && dealer == Owner.Creature ? SpecializationBonus : 0;
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        return Specialization == ElesisSpecialization.SoarKnight && target == Owner.Creature ? SpecializationBonus : 0;
    }

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != Owner || room == null || !ElesisSpecializationController.IsCombatExperienceNode(room.RoomType))
        {
            MainFile.Logger.Info($"Elesis XP reward not added. playerMatches={player == Owner} room={room?.RoomType.ToString() ?? "null"}");
            return false;
        }

        if (rewards.OfType<ElesisExperienceReward>().Any())
        {
            MainFile.Logger.Info($"Elesis XP reward already present. room={room.RoomType}");
            return false;
        }

        var amount = ElesisSpecializationController.ExperienceFor(room.RoomType);
        MainFile.Logger.Info($"Adding Elesis XP reward. room={room.RoomType} amount={amount}");
        rewards.Add(new ElesisExperienceReward(player, amount));
        return true;
    }
}
