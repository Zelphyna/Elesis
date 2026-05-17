using Godot;
using HarmonyLib;
using BaseLib.Abstracts;
using Elesis.ElesisCode.Character;
using Elesis.ElesisCode.Events;
using Elesis.ElesisCode.Rewards;
using Elesis.ElesisCode.Specializations;
using MegaCrit.Sts2.Core.Modding;

namespace Elesis.ElesisCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Elesis";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        _ = new ElesisSpecializationEvent();
        _ = new ElesisSecondEvolutionEvent();
        _ = new ElesisFinalEvolutionEvent();
        new ElesisExperienceReward(null!, 0).Initialize();
        ElesisSpecializationVisuals.RegisterSceneConversions();
        ModelDbCustomCharacters.Register(new Character.Elesis());
    }
}
