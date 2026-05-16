using BaseLib.Abstracts;
using BaseLib.Utils;
using Elesis.ElesisCode.Cards;
using Elesis.ElesisCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Elesis.ElesisCode.Cards.Basic;

[Pool(typeof(ElesisCardPool))]
public sealed class ElesisDefend() : ElesisCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override IEnumerable<CardKeyword> ElesisCardKeywords => [ElesisKeywords.Chivalry];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await ElesisMechanics.GainChivalry(Owner.Creature, 1m, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
