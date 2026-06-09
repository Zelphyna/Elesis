using BaseLib.Abstracts;
using Elesis.ElesisCode.Extensions;
using Godot;

namespace Elesis.ElesisCode.Character;

public class ElesisCardPool : CustomCardPoolModel
{
    public override string Title => Elesis.CharacterId;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    public override Color ShaderColor => Elesis.Color;

    public override Color DeckEntryCardColor => Elesis.Color;
    public override bool IsColorless => false;
}
