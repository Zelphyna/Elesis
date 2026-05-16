using BaseLib.Abstracts;
using Elesis.ElesisCode.Extensions;
using Godot;

namespace Elesis.ElesisCode.Character;

public class ElesisPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Elesis.Color;
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
