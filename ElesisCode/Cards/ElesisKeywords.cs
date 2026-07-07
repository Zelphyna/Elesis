using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Elesis.ElesisCode.Cards;

public static class ElesisKeywords
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword CounterAttack;
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)] public static CardKeyword Burn;

    public static IEnumerable<CardKeyword> ForCounterAttack() => [CounterAttack];

    public static IEnumerable<CardKeyword> ForBurn() => [Burn];
}
