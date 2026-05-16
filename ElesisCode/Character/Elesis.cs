using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Elesis.ElesisCode.Cards;
using Elesis.ElesisCode.Cards.Basic;
using Elesis.ElesisCode.Extensions;
using Elesis.ElesisCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Elesis.ElesisCode.Character;

public class Elesis : CustomCharacterModel
{
    public const string CharacterId = "Elesis";

    public static readonly Color Color = new("aab2ff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;
    public override bool HideFromVanillaCharacterSelect => false;
    public override bool AllowInVanillaRandomCharacterSelect => true;
    public override string CharacterSelectSfx => "";
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
    public override string CustomAttackSfx => "event:/sfx/characters/ironclad/ironclad_attack";
    public override string CustomCastSfx => "event:/sfx/characters/ironclad/ironclad_cast";
    public override string CustomDeathSfx => "event:/sfx/characters/ironclad/ironclad_die";
    public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";
    public override string CustomVisualPath => $"{MainFile.ResPath}/scenes/creature_visuals/elesis_combat.tscn";
    public override string CustomMerchantAnimPath => SceneHelper.GetScenePath("merchant/characters/ironclad_merchant");
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/scenes/rest_site/elesis_rest.tscn";
    public override string CustomTrailPath => "res://scenes/vfx/card_trail_ironclad.tscn";
    public override string CustomEnergyCounterPath => $"{MainFile.ResPath}/scenes/combat/elesis_energy_counter.tscn";
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconPath => SceneHelper.GetScenePath("ui/character_icons/ironclad_icon");
    public override string CustomIconOutlineTexturePath => "elesis_icon.png".CharacterUiPath();
    public override string CustomArmPointingTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_ironclad_point.png");
    public override string CustomArmRockTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_ironclad_rock.png");
    public override string CustomArmPaperTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_ironclad_paper.png");
    public override string CustomArmScissorsTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_ironclad_scissors.png");

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<ElesisStrike>(),
        ModelDb.Card<ElesisStrike>(),
        ModelDb.Card<ElesisStrike>(),
        ModelDb.Card<ElesisStrike>(),
        ModelDb.Card<ElesisDefend>(),
        ModelDb.Card<ElesisDefend>(),
        ModelDb.Card<ElesisDefend>(),
        ModelDb.Card<ElesisDefend>(),
        ModelDb.Card<QuickStep>(),
        ModelDb.Card<ClaymoreArc>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<BelderKnightEmblem>()
    ];

    public override List<string> GetArchitectAttackVfx() => [];

    public override CardPoolModel CardPool => ModelDb.CardPool<ElesisCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ElesisRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ElesisPotionPool>();

    public override string CustomIconTexturePath => "elesis_icon.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "elesis_select.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "elesis_select_locked.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/scenes/character_select/elesis_character_select_bg.tscn";
    public override string CustomMapMarkerPath => "elesis_icon.png".CharacterUiPath();
}
