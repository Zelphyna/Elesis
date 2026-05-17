using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;
using Elesis.ElesisCode.Relics;

namespace Elesis.ElesisCode.Specializations;

public static class ElesisSpecializationVisuals
{
    private const string BaseCombatScene = $"{MainFile.ResPath}/scenes/creature_visuals/elesis_combat.tscn";

    public static void RegisterSceneConversions()
    {
        BaseCombatScene.RegisterSceneForConversion<NCreatureVisuals>();
        SceneFor(ElesisSpecialization.SaberKnight).RegisterSceneForConversion<NCreatureVisuals>();
        SceneFor(ElesisSpecialization.PyroKnight).RegisterSceneForConversion<NCreatureVisuals>();
        SceneFor(ElesisSpecialization.DarkKnight).RegisterSceneForConversion<NCreatureVisuals>();
        SceneFor(ElesisSpecialization.SoarKnight).RegisterSceneForConversion<NCreatureVisuals>();
    }

    public static string CurrentCombatScenePath()
    {
        var state = RunManager.Instance.DebugOnlyGetState();
        if (state == null)
        {
            return BaseCombatScene;
        }

        var player = LocalContext.GetMe(state);
        var emblem = player?.Relics.OfType<BelderKnightEmblem>().FirstOrDefault();
        return emblem == null ? BaseCombatScene : SceneFor(emblem.Specialization);
    }

    public static string SceneFor(ElesisSpecialization specialization)
    {
        return specialization switch
        {
            ElesisSpecialization.SaberKnight => $"{MainFile.ResPath}/scenes/creature_visuals/specializations/elesis_saber_knight_combat.tscn",
            ElesisSpecialization.PyroKnight => $"{MainFile.ResPath}/scenes/creature_visuals/specializations/elesis_pyro_knight_combat.tscn",
            ElesisSpecialization.DarkKnight => $"{MainFile.ResPath}/scenes/creature_visuals/specializations/elesis_dark_knight_combat.tscn",
            ElesisSpecialization.SoarKnight => $"{MainFile.ResPath}/scenes/creature_visuals/specializations/elesis_soar_knight_combat.tscn",
            _ => BaseCombatScene
        };
    }
}
