using BaseLib.Abstracts;
using Elesis.ElesisCode.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Elesis.ElesisCode.Rewards;

public sealed class ElesisExperienceReward : CustomReward
{
    public const int RewardTypeValue = 9501;
    public static readonly RewardType ExperienceRewardType = (RewardType)RewardTypeValue;

    public ElesisExperienceReward(Player player, int amount) : base(player)
    {
        Amount = amount;
    }

    public int Amount { get; }

    protected override RewardType RewardType => ExperienceRewardType;
    protected override string IconPath => $"{MainFile.ResPath}/images/relics/belder_knight_emblem.png";
    public override bool IsPopulated => true;
    public override CreateRewardFromSave<CustomReward> DeserializeMethod => CreateFromSerializable;

    public override LocString Description
    {
        get
        {
            var loc = GetLoc();
            loc.Add("XP", Amount);
            return loc;
        }
    }

    public override void Populate()
    {
    }

    protected override Task<bool> OnSelect()
    {
        var emblem = Player.Relics.OfType<BelderKnightEmblem>().FirstOrDefault();
        if (emblem == null)
        {
            MainFile.Logger.Info("Elesis XP reward selected but Belder Knight Emblem was not found.");
            return Task.FromResult(false);
        }

        emblem.ClaimCombatExperienceReward(Amount);
        return Task.FromResult(true);
    }

    public override void MarkContentAsSeen()
    {
    }

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward
        {
            RewardType = RewardType,
            GoldAmount = Amount
        };
    }

    public static CustomReward CreateFromSerializable(SerializableReward save, Player player)
    {
        return new ElesisExperienceReward(player, save.GoldAmount);
    }
}
