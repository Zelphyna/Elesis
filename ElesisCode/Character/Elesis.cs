using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Elesis.ElesisCode.Cards.Basic;
using Elesis.ElesisCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Elesis.ElesisCode.Character;

public class Elesis : PlaceholderCharacterModel
{
    public const string CharacterId = "Elesis";

    public static readonly Color Color = new("aab2ff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 70;
    public override bool HideFromVanillaCharacterSelect => false;
    public override bool AllowInVanillaRandomCharacterSelect => true;

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<ElesisStrike>(),
        ModelDb.Card<ElesisStrike>(),
        ModelDb.Card<ElesisStrike>(),
        ModelDb.Card<ElesisStrike>(),
        ModelDb.Card<ElesisDefend>(),
        ModelDb.Card<ElesisDefend>(),
        ModelDb.Card<ElesisDefend>(),
        ModelDb.Card<ElesisDefend>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [];

    public override CardPoolModel CardPool => ModelDb.CardPool<ElesisCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ElesisRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ElesisPotionPool>();

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "elesis_icon.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "elesis_select.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "elesis_select_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "elesis_map_marker.png".CharacterUiPath();
}
