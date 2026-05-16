using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Elesis.ElesisCode.Cards;

public static class ElesisKeywords
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Vitality;
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Destruction;
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Flame;
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Parry;
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Chivalry;

    public static IEnumerable<CardKeyword> For(ElesisStyle style, decimal chivalryGain, decimal flameGain)
    {
        switch (style)
        {
            case ElesisStyle.Vitality:
                yield return Vitality;
                yield return Chivalry;
                break;
            case ElesisStyle.Destruction:
                yield return Destruction;
                yield return Chivalry;
                break;
            case ElesisStyle.Flame:
                yield return Flame;
                break;
            case ElesisStyle.Parry:
                yield return Parry;
                yield return Chivalry;
                break;
        }

        if (style == ElesisStyle.None && chivalryGain > 0m)
        {
            yield return Chivalry;
        }

        if (style != ElesisStyle.Flame && flameGain > 0m)
        {
            yield return Flame;
        }
    }
}
