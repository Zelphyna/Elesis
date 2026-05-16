using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Character;

namespace Elesis.ElesisCode.Potions;

[Pool(typeof(ElesisPotionPool))]
public abstract class ElesisPotion : CustomPotionModel;
