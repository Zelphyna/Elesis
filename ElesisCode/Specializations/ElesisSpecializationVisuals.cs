using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;
using Elesis.ElesisCode.Relics;

namespace Elesis.ElesisCode.Specializations;

public static class ElesisSpecializationVisuals
{
    private const string BaseCombatScene = $"{MainFile.ResPath}/scenes/creature_visuals/elesis_combat.tscn";
    private const string BaseMerchantScene = $"{MainFile.ResPath}/scenes/merchant/elesis_shop.tscn";
    private const string BaseRestSiteScene = $"{MainFile.ResPath}/scenes/rest_site/elesis_rest.tscn";

    public static void RegisterSceneConversions()
    {
        BaseCombatScene.RegisterSceneForConversion<NCreatureVisuals>();
        foreach (var specialization in Branches())
        {
            for (var tier = 1; tier <= 3; tier++)
            {
                SceneFor(specialization, tier).RegisterSceneForConversion<NCreatureVisuals>();
            }
        }
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
        return emblem == null ? BaseCombatScene : SceneFor(emblem.Specialization, emblem.EvolutionTier);
    }

    public static string CurrentMerchantScenePath()
    {
        var state = RunManager.Instance.DebugOnlyGetState();
        if (state == null)
        {
            return BaseMerchantScene;
        }

        var player = LocalContext.GetMe(state);
        var emblem = player?.Relics.OfType<BelderKnightEmblem>().FirstOrDefault();
        return emblem == null ? BaseMerchantScene : MerchantSceneFor(emblem.Specialization, emblem.EvolutionTier);
    }

    public static string CurrentRestSiteScenePath()
    {
        var state = RunManager.Instance.DebugOnlyGetState();
        if (state == null)
        {
            return BaseRestSiteScene;
        }

        var player = LocalContext.GetMe(state);
        var emblem = player?.Relics.OfType<BelderKnightEmblem>().FirstOrDefault();
        return emblem == null ? BaseRestSiteScene : RestSiteSceneFor(emblem.Specialization, emblem.EvolutionTier);
    }

    public static string SceneFor(ElesisSpecialization specialization, int tier)
    {
        return ImageNameFor(specialization, tier) is { } imageName
            ? $"{MainFile.ResPath}/scenes/creature_visuals/specializations/elesis_{imageName}_combat.tscn"
            : BaseCombatScene;
    }

    public static string MerchantSceneFor(ElesisSpecialization specialization, int tier)
    {
        return ImageNameFor(specialization, tier) is { } imageName
            ? $"{MainFile.ResPath}/scenes/merchant/specializations/elesis_{imageName}_shop.tscn"
            : BaseMerchantScene;
    }

    public static string RestSiteSceneFor(ElesisSpecialization specialization, int tier)
    {
        return ImageNameFor(specialization, tier) is { } imageName
            ? $"{MainFile.ResPath}/scenes/rest_site/specializations/elesis_{imageName}_rest.tscn"
            : BaseRestSiteScene;
    }

    public static string? ImagePathFor(ElesisSpecialization specialization, int tier)
    {
        return ImageNameFor(specialization, tier) is { } imageName
            ? $"{MainFile.ResPath}/images/specializations/{imageName}.png"
            : null;
    }

    public static string DisplayNameFor(ElesisSpecialization specialization, int tier)
    {
        return ImageNameFor(specialization, tier)?.Replace('_', ' ') ?? "Elesis";
    }

    private static IReadOnlyList<ElesisSpecialization> Branches()
    {
        return
        [
            ElesisSpecialization.SaberKnight,
            ElesisSpecialization.PyroKnight,
            ElesisSpecialization.DarkKnight,
            ElesisSpecialization.SoarKnight
        ];
    }

    private static string? ImageNameFor(ElesisSpecialization specialization, int tier)
    {
        return (specialization, Math.Clamp(tier, 1, 3)) switch
        {
            (ElesisSpecialization.SaberKnight, 1) => "saber_knight",
            (ElesisSpecialization.SaberKnight, 2) => "grand_master",
            (ElesisSpecialization.SaberKnight, 3) => "empire_sword",
            (ElesisSpecialization.PyroKnight, 1) => "pyro_knight",
            (ElesisSpecialization.PyroKnight, 2) => "blazing_heart",
            (ElesisSpecialization.PyroKnight, 3) => "flame_lord",
            (ElesisSpecialization.DarkKnight, 1) => "dark_knight",
            (ElesisSpecialization.DarkKnight, 2) => "crimson_avenger",
            (ElesisSpecialization.DarkKnight, 3) => "bloody_queen",
            (ElesisSpecialization.SoarKnight, 1) => "soar_knight",
            (ElesisSpecialization.SoarKnight, 2) => "patrona",
            (ElesisSpecialization.SoarKnight, 3) => "adrestia",
            _ => null
        };
    }
}
