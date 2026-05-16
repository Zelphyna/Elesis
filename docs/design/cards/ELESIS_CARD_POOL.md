# Elesis Card Pool

Prototype card pool: 40 implemented cards.

Current implementation status:

- Cards are playable prototype cards using confirmed template-safe effects: damage, block, cost, rarity, targeting, upgrades, draw, and powers.
- `Chivalry` and `Flame` are implemented as visible counter powers.
- `Vitality` cards gain Chivalry. When Vitality resolves at 5 or more Chivalry, it spends 5 Chivalry to draw 1 card.
- `Destruction` cards spend 5 Chivalry for bonus damage, and mixed attack/block Destruction cards also gain bonus Block.
- `Flame` cards generate Flame. Non-Flame Attacks consume all Flame and add that much damage.
- `Parry` cards currently gain Chivalry and provide efficient Block; dedicated counterattack behavior is still open.
- Prototype cards share the placeholder card art until dedicated assets are produced.
- Starting deck is 10 cards: 4 `ElesisStrike`, 4 `ElesisDefend`, 1 `QuickStep`, and 1 `ClaymoreArc`. `QuickStep` introduces Vitality/Chivalry cycling, while `ClaymoreArc` gives the starter deck a first Destruction payoff once Chivalry reaches 5.

## Basic

- `ElesisStrike`: 1-cost Attack, 6 damage, upgrades by 3.
- `ElesisDefend`: 1-cost Skill, 5 Block, upgrades by 3.

## Common

- `QuickStep`: 0-cost Skill, 3 Block, Vitality label.
- `RedTempo`: 0-cost Attack, 4 damage, Vitality label.
- `RisingCut`: 1-cost Attack, 8 damage.
- `GuardingSlash`: 1-cost Attack, 5 damage and 5 Block.
- `IronFootwork`: 1-cost Skill, 8 Block, Vitality label.
- `EmberCut`: 1-cost Attack, 7 damage, Flame label.
- `SwordPressure`: 1-cost Attack, 9 damage.
- `ForwardGuard`: 1-cost Skill, 6 Block.
- `VitalLunge`: 1-cost Attack, 6 damage and 4 Block, Vitality label.
- `FlameTap`: 0-cost Attack, 3 damage, Flame label.
- `ClaymoreArc`: 2-cost Attack, 15 damage, Destruction label.
- `BelderDiscipline`: 2-cost Skill, 13 Block.

## Uncommon

- `DestructionBlow`: 2-cost Attack, 18 damage, Destruction label.
- `VitalityRush`: 1-cost Attack, 7 damage and 7 Block, Vitality label.
- `FlameGuard`: 1-cost Attack, 6 damage and 8 Block, Flame label.
- `CounterStance`: 1-cost Skill, 10 Block, Parry label.
- `SpiralBlade`: 2-cost Attack, 21 damage, Destruction label.
- `IgnitionEdge`: 1-cost Attack, 10 damage, Flame label.
- `BreakingCharge`: 2-cost Attack, 14 damage and 8 Block, Destruction label.
- `KnightlyResolve`: 2-cost Skill, 16 Block.
- `HeavyCleave`: 2-cost Attack, 20 damage, Destruction label.
- `BlazingAdvance`: 1-cost Attack, 8 damage and 6 Block, Flame label.
- `RedComet`: 2-cost Attack, 24 damage, Flame label.
- `DuelistsGuard`: 1-cost Skill, 11 Block, Parry label.
- `FlameWheel`: 2-cost Attack, 17 damage, Flame label.

## Rare

- `RoyalAssault`: 3-cost Attack, 32 damage, Destruction label.
- `CrimsonOath`: 2-cost Attack, 18 damage and 14 Block.
- `FinalIgnition`: 3-cost Attack, 36 damage, Flame label.
- `UnbrokenKnight`: 2-cost Skill, 22 Block.
- `SwordOfBelder`: 2-cost Attack, 26 damage.
- `PhoenixStep`: 1-cost Attack, 11 damage and 11 Block, Flame label.
- `ScarletJudgment`: 3-cost Attack, 40 damage, Destruction label.
- `KnightCaptain`: 1-cost Skill, 14 Block.
- `BurningResolve`: 2-cost Attack, 16 damage and 16 Block, Flame label.
- `ElLadyEcho`: 2-cost Attack, 28 damage.
- `GrandCrossCut`: 3-cost Attack, 44 damage, Destruction label.
- `CrimsonFinale`: 3-cost Attack, 30 damage and 18 Block.
- `ElswordLegacy`: 3-cost Attack, 48 damage, Destruction label.
