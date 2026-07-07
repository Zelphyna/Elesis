# Elesis Card Pool

Prototype card pool: 40 implemented cards.

Current implementation status:

- Cards are playable prototype cards using confirmed template-safe effects: damage, block, cost, rarity, targeting, upgrades, draw, and powers.
- `Chivalry` and `Flame` are implemented as visible counter powers.
- `Vitality` cards gain Chivalry. When Vitality resolves at 5 or more Chivalry, it spends 5 Chivalry to draw; dedicated draw cards can use that spend for a larger bonus draw.
- `Destruction` cards spend 5 Chivalry for bonus damage, and mixed attack/block Destruction cards also gain bonus Block.
- `Flame` cards generate Flame. Non-Flame Attacks consume all Flame and add that much damage.
- `Parry` cards currently gain Chivalry and provide efficient Block; dedicated counterattack behavior is still open.
- Card descriptions list direct damage, Block, or draw before keyword-specific mechanics.
- Prototype cards share the placeholder card art until dedicated assets are produced.
- Starting deck is 10 cards: 4 `ElesisStrike`, 4 `ElesisDefend`, 1 `QuickStep`, and 1 `ClaymoreArc`. `QuickStep` introduces Vitality/Chivalry cycling, while `ClaymoreArc` gives the starter deck a first Destruction payoff once Chivalry reaches 5.

## STS2 Vanilla Pool Baseline

Confirmed from the local Slay the Spire 2 `v0.103.2` assembly (`release_info.json` commit `89765e1e`, dated 2026-04-16). The counted vanilla character pools are `IroncladCardPool`, `SilentCardPool`, `DefectCardPool`, `NecrobinderCardPool`, and `RegentCardPool`.

Vanilla pool size:

| Character | Total | Basic | Common | Uncommon | Rare | Ancient |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Ironclad | 87 | 3 | 20 | 36 | 26 | 2 |
| Silent | 88 | 4 | 20 | 36 | 26 | 2 |
| Defect | 88 | 4 | 20 | 36 | 26 | 2 |
| Necrobinder | 88 | 4 | 20 | 36 | 26 | 2 |
| Regent | 88 | 4 | 20 | 36 | 26 | 2 |

Draftable Common/Uncommon/Rare type counts:

| Character | Draftable total | Attacks | Skills | Powers |
| --- | ---: | ---: | ---: | ---: |
| Ironclad | 82 | 34 | 28 | 20 |
| Silent | 82 | 25 | 39 | 18 |
| Defect | 82 | 28 | 35 | 19 |
| Necrobinder | 82 | 32 | 33 | 17 |
| Regent | 82 | 29 | 35 | 18 |
| Vanilla median target | 82 | 29 | 35 | 18 |

Rarity/type shape by character:

| Character | Common A/S/P | Uncommon A/S/P | Rare A/S/P |
| --- | ---: | ---: | ---: |
| Ironclad | 13 / 7 / 0 | 14 / 13 / 9 | 7 / 8 / 11 |
| Silent | 9 / 11 / 0 | 11 / 17 / 8 | 5 / 11 / 10 |
| Defect | 12 / 8 / 0 | 8 / 18 / 10 | 8 / 9 / 9 |
| Necrobinder | 12 / 8 / 0 | 12 / 15 / 9 | 8 / 10 / 8 |
| Regent | 9 / 11 / 0 | 11 / 17 / 8 | 9 / 7 / 10 |
| Median target | 11 / 8 / 0 | 11 / 17 / 9 | 8 / 9 / 10 |

Elesis current shape:

| Scope | Total | Attacks | Skills | Powers |
| --- | ---: | ---: | ---: | ---: |
| All implemented | 40 | 26 | 11 | 3 |
| Draftable Common/Uncommon/Rare | 36 | 24 | 9 | 3 |

Elesis current rarity/type shape:

| Rarity | Attacks | Skills | Powers | Total |
| --- | ---: | ---: | ---: | ---: |
| Basic | 2 | 2 | 0 | 4 |
| Common | 5 | 4 | 1 | 10 |
| Uncommon | 10 | 2 | 1 | 13 |
| Rare | 9 | 3 | 1 | 13 |

Design target for parity:

- Keep the 10-card starting deck shape unless a playtest proves it should change: 4 Strikes, 4 Defends, and 2 character-defining Basic cards.
- Bring the draftable pool toward 82 cards: 20 Common, 36 Uncommon, 26 Rare. Ancient cards are optional until the unlock/timeline purpose is confirmed.
- Use the vanilla median as the default target: about 29 Attacks, 35 Skills, and 18 Powers across Common/Uncommon/Rare.
- For Elesis, that means adding roughly 46 draftable cards: about 5 Attacks, 26 Skills, and 15 Powers.
- Do not add Common Powers just to fill quota. Vanilla character pools have 0 Common Powers in `v0.103.2`; place most Powers at Uncommon and Rare.
- Recommended next content target:
  - Common: add 10 cards, aiming for +6 Attacks and +4 Skills, keeping Common Powers at 1 only if `BelderDiscipline` remains Common for mechanic onboarding.
  - Uncommon: add 23 cards, aiming for about +1 Attack, +15 Skills, +7 Powers.
  - Rare: add 13 cards, aiming for about +6 Skills and +7 Powers; avoid adding more Rare Attacks unless an existing Rare Attack is moved or cut.

Confirmed implication: Elesis currently reads as an attack-heavy prototype, not a full STS2-style character pool. The next balance pass should primarily create Skills and Powers that support Chivalry, Flame, Destruction, and Parry decisions rather than more direct damage cards.

## Basic

- `ElesisStrike`: 1-cost Attack, 2 damage 3 times, upgrades by 1 per hit.
- `ElesisDefend`: 1-cost Skill, 5 Block, upgrades by 3.

## Common

- `QuickStep`: 0-cost Skill, 3 Block, Vitality label.
- `RedTempo`: 0-cost Skill, draw 1, Vitality label; spending 5 Chivalry draws 1 additional card.
- `RisingCut`: 1-cost Attack, 4 damage 2 times.
- `GuardingSlash`: 1-cost Attack, 5 damage and 5 Block.
- `IronFootwork`: 1-cost Skill, 8 Block, Vitality label.
- `EmberCut`: 1-cost Attack, 2 damage 3 times, Flame label.
- `SwordPressure`: 1-cost Skill, draw 2, Vitality label; spending 5 Chivalry draws 1 additional card.
- `ForwardGuard`: 1-cost Skill, 6 Block.
- `VitalLunge`: 1-cost Attack, 6 damage and 4 Block, Vitality label.
- `FlameTap`: 0-cost Attack, 1 damage 2 times, Flame label.
- `ClaymoreArc`: 2-cost Attack, 15 damage, Destruction label.
- `BelderDiscipline`: 1-cost Power, start-of-turn Chivalry flow.

## Uncommon

- `DestructionBlow`: 2-cost Attack, 18 damage, Destruction label.
- `VitalityRush`: 1-cost Attack, 7 damage and 7 Block, Vitality label.
- `FlameGuard`: 1-cost Attack, 6 damage and 8 Block, Flame label.
- `CounterStance`: 1-cost Power, start-of-turn Chivalry and Flame flow, Parry label.
- `SpiralBlade`: 2-cost Attack, 21 damage, Destruction label.
- `IgnitionEdge`: 1-cost Attack, 10 damage, Flame label.
- `BreakingCharge`: 2-cost Attack, 14 damage and 8 Block, Destruction label.
- `KnightlyResolve`: 2-cost Skill, 16 Block.
- `HeavyCleave`: 2-cost Attack, 20 damage, Destruction label.
- `BlazingAdvance`: 1-cost Attack, 8 damage and 6 Block, Flame label.
- `RedComet`: 2-cost Attack, 6 damage 3 times, Flame label.
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
- `BurningResolve`: 1-cost Power, start-of-turn Flame flow, Flame label.
- `ElLadyEcho`: 2-cost Skill, draw 3, Vitality label; spending 5 Chivalry draws 2 additional cards.
- `GrandCrossCut`: 3-cost Attack, 44 damage, Destruction label.
- `CrimsonFinale`: 3-cost Attack, 30 damage and 18 Block.
- `ElswordLegacy`: 3-cost Attack, 48 damage, Destruction label.
