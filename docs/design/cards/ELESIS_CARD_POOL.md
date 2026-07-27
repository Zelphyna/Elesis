# Elesis Card Pool

Implemented pool: **88 cards**. The draftable Common/Uncommon/Rare pool contains **82 cards**: 29 Attacks, 35 Skills, and 18 Powers.

| Rarity | Attack | Skill | Power | Total |
|---|---:|---:|---:|---:|
| Basic | 1 | 3 | 0 | 4 |
| Common | 11 | 9 | 0 | 20 |
| Uncommon | 10 | 17 | 9 | 36 |
| Rare | 8 | 9 | 9 | 26 |
| Ancient | 1 | 1 | 0 | 2 |
| **Total** | **31** | **39** | **18** | **88** |

## Mechanical Contract

- **Counter Attack** is a visible player power. Each enemy attack hit received deals the current Counter Attack amount back as fixed, blockable, unpowered damage, including a fully blocked hit. It normally expires at Block cleanup.
- **Burn** is an enemy debuff. Before its monster acts, Burn deals its full amount as unblockable, unpowered damage, then loses half its charges rounded up unless a Power changes the decay rule.
- An immediate Burn trigger deals a stated fraction of the current stack without reducing it unless the card explicitly says otherwise.
- Fractional damage is rounded down; Burn loss is rounded up.
- The values below are the implemented first-playtest targets, not a claim of final balance.

Starting deck: 4 Elesis Strike, 4 Elesis Defend, 1 Counter Guard, and 1 Burning Edge.

## Basic — 4

| Card | Cost / Type | Base | Upgrade |
|---|---|---|---|
| Elesis Strike | 1 Attack | Deal 6. | Deal 9. |
| Elesis Defend | 1 Skill | Gain 5 Block. | Gain 8 Block. |
| Counter Guard | 1 Skill | Gain 6 Counter Attack. | Gain 9. |
| Burning Edge | 1 Skill | Apply 5 Burn. | Apply 8. |

## Common — 20

| Card | Cost / Type | Base | Upgrade |
|---|---|---|---|
| Measured Slash | 1 Attack | Deal 7; if target attacks, gain 3 Counter. | 9 damage; 4 Counter. |
| Ember Thrust | 1 Attack | Deal 6; apply 3 Burn. | 8 damage; 4 Burn. |
| Scorching Feint | 0 Attack | Deal 3; if target has Burn, gain 3 Block. | 5 damage; 4 Block. |
| Guarded Cut | 1 Attack | Deal 6; gain 4 Block, +2 with Counter. | 8 damage; 5 Block, +3. |
| Kindled Sweep | 1 Attack | Deal 5 and apply 2 Burn to all. | 7 damage; 3 Burn. |
| Answering Blow | 1 Attack | Deal 8; draw 1 with Counter. | Deal 11. |
| Coalbrand | 1 Attack | Deal 9; apply 4 Burn if target has none. | 12 damage; 5 Burn. |
| Opening Read | 1 Attack | Deal 7; gain 3 Counter if target attacks, otherwise apply 3 Burn. | 9 damage; 4 Counter/Burn. |
| Searing Pommel | 1 Attack | Deal 8; draw 1 if target has Burn. | Deal 11. |
| Crossfire Slash | 2 Attack | Deal 6 twice; apply 3 Burn. | 8 twice; 4 Burn. |
| Challenge Sweep | 2 Attack | Deal 11 to all; gain 5 Counter. | 14 damage; 7 Counter. |
| Ready Guard | 1 Skill | Gain 6 Block and 3 Counter. | 8 Block; 4 Counter. |
| Heat Haze | 1 Skill | Apply 4 Burn to all. | Apply 6. |
| Low Guard | 0 Skill | Gain 4 Block; gain 2 Counter if any enemy attacks. | 6 Block; 3 Counter. |
| Ashen Veil | 1 Skill | Gain 7 Block, +3 if any enemy has Burn. | 10 Block, +4. |
| Red Challenge | 1 Skill | Apply 1 Weak; gain 4 Counter. | 2 Weak; 5 Counter. |
| Banked Spark | 1 Skill | Apply 7 Burn. | Apply 10. |
| Steady Breath | 1 Skill | Draw 1; gain 4 Counter. | Draw 2; gain 5. |
| Covering Embers | 1 Skill | Gain 5 Block; apply 3 Burn to all. | 8 Block; 4 Burn. |
| Rooted Defense | 1 Skill, Retain | Gain 8 Block. | Gain 12. |

## Uncommon — 36

| Card | Cost / Type | Base | Upgrade |
|---|---|---|---|
| Reprisal Cut | 1 Attack | Deal 9; next Counter trigger gains +5 damage. | 12 damage; +8. |
| Burning Reversal | 1 Attack | Deal 9; gain 6 Counter if target has Burn. | 12 damage; 9 Counter. |
| Cinder Barrage | 2 Attack | Deal 4 three times; apply 1 Burn after each. | Deal 5 three times. |
| Pressure Break | 2 Attack | Deal 15; apply 2 Vulnerable with Counter. | 19 damage; 3 Vulnerable. |
| Flashpoint | 1 Attack | Deal 7, then half target Burn without decay. | Deal 10 first. |
| Backstep Cleave | 1 Attack | Deal 8 to all; gain 5 Counter if any enemy attacks. | 11 damage; 8 Counter. |
| Tempered Edge | 1 Attack | Deal 10, +1 per 3 target Burn. | 14 damage; +1 per 2. |
| Provoked Assault | 2 Attack | Deal 16; gain 2 Counter per intent hit, cap 10. | 21 damage; 3 per hit, cap 15. |
| Charred Wound | 1 Attack | Deal 8; add half current Burn, cap 8. | 11 damage; cap 12. |
| Twin Verdict | 2 Attack | Deal 9 twice; 3 Burn after first, 4 Counter after second. | 12 twice; 4 Burn; 6 Counter. |
| Layered Defense | 1 Skill | 4 Block/Counter, +2 each per attacking enemy. | 6 each, +3. |
| Countermeasure | 1 Skill | Gain 9 Counter; draw 1 if already active. | Gain 13. |
| Shelter in Sparks | 1 Skill | Gain 9 Block; gain 5 Counter if any enemy has Burn. | 12 Block; 7 Counter. |
| Fan the Ashes | 1 Skill | Apply 6 Burn to all; next decay loses 2 fewer. | 8 Burn; loses 3 fewer. |
| Transfer Heat | 0 Skill, Exhaust | Convert up to 6 enemy Burn into Counter. | Convert up to 9. |
| Redirection | 1 Skill | Gain 8 Block; next enemy hit applies 5 Burn to its attacker. | 11 Block; 8 Burn. |
| Calculated Risk | 0 Skill, Exhaust | Lose 3 HP; gain 10 Counter. | Lose 2; gain 14. |
| Cinder Screen | 1 Skill | Gain 5 Block, +3 per burned enemy. | 7 Block, +4. |
| Burning Patience | 1 Skill, Retain | Apply 10 Burn. | Apply 14. |
| Steel Nerves | 1 Skill | Gain 12 Block; if no enemy attacks, apply 5 Burn to all. | 16 Block; 7 Burn. |
| Echo Guard | 2 Skill | 13 Block, 8 Counter; next hit triggers Counter twice. | 16 Block, 10 Counter; next 2 hits. |
| Ash Reclamation | 1 Skill | Remove target Burn; draw 1 per 5, cap 3. | 1 per 4, cap 4. |
| Shared Threat | 1 Skill | Copy target Burn as Counter, cap 12. | Cap 18. |
| Watch the Blade | 1 Skill | Draw 2; gain 6 Counter if any enemy attacks. | Gain 10. |
| Smoldering Guard | 1 Skill | Gain 9 Block; next block-breaking hit burns attacker for 6. | 13 Block; 9 Burn. |
| Delay the Strike | 1 Skill | Apply 2 Weak; if target attacks, apply 7 Burn. | 3 Weak; 10 Burn. |
| Tactical Withdrawal | 0 Skill, Exhaust | Draw 1; retain up to 7 Counter at next cleanup. | Retain up to 12. |
| Ever Ready | 1 Power | Start turn: gain 2 Counter. | Gain 3. |
| Cinder Etching | 1 Power | First Attack hit each turn applies 2 Burn. | Apply 3. |
| Tempered Retort | 1 Power | First Counter damage to each enemy each enemy turn applies 3 Burn. | Apply 5. |
| Aegis Teeth | 1 Power | Each card that grants Block also grants 1 Counter once. | Grant 2. |
| Ashen Shelter | 1 Power | Each Burn damage trigger grants 3 Block. | Grant 5. |
| Punishing Rhythm | 1 Power | After first received hit each enemy turn, gain 3 Counter. | Gain 5. |
| Deep Scorch | 1 Power | First Burn application each turn gains +3 Burn. | +5. |
| Falling Embers | 1 Power | After Burn decay, move 2 Burn to another enemy, or reapply 1. | Move 3, or reapply 2. |
| Furnace Rampart | 2 Power | Enemy turn start: with Counter, gain 7 Block. | Gain 10. |

## Rare — 26

| Card | Cost / Type | Base | Upgrade |
|---|---|---|---|
| Crimson Reprisal | 2 Attack | Deal 14; if Counter dealt damage since last turn, repeat once. | 18 damage; repeat twice. |
| Pyre Divide | 2 Attack | Deal 14; trigger full target Burn without decay. | Deal 19 first. |
| Perfect Answer | 2 Attack | Deal 20; gain half unblocked damage as Counter, cap 12. | 25 damage; cap 18. |
| Scarlet Crossfire | 3 Attack | Deal 9 three times; apply 2 Burn after each. | 12 three times; 3 Burn. |
| Sentence of Ash | 2 Attack | Deal 13 to all; burned enemies take extra up to 18. | 18 damage; cap 27. |
| Mirrorsteel Lunge | 1 Attack | Deal 12; next hit triggers Counter twice. | 16 damage; next 2 hits. |
| Lasting Scar | 1 Attack, Exhaust | Deal 9; double target Burn, adding up to 20. | 13 damage; cap 30. |
| Red Horizon | 3 Attack, Exhaust | Deal 26 to all; gain 12 Counter; apply 8 Burn to all. | 34 damage; 17 Counter; 12 Burn. |
| Absolute Guard | 2 Skill | Gain 22 Block and 15 Counter. | 30 Block; 21 Counter. |
| Sealed Defense | 1 Skill, Exhaust | Gain 13 Block; Counter survives next cleanup. | 18 Block; no Exhaust. |
| Ash Cascade | 2 Skill | Apply 14 Burn to all, then trigger half without decay. | Apply 18. |
| Borrowed Heat | 1 Skill | Gain target Burn as Block, cap 22; remove half Burn. | Cap 32; remove one third. |
| Return to Sender | 1 Skill | Per target intent hit, cap 5: gain 4 Block and Counter. | Gain 5 each. |
| Brand the Aggressor | 1 Skill | Apply 11 Burn; its attack hits add 3 Burn before next turn. | 15 initial; 5 per hit. |
| Encircled Bulwark | 2 Skill | 8 Block/Counter, +5 per additional attacking enemy. | 11 each, +7. |
| Rekindled Defense | 1 Skill, Exhaust | Remove all enemy Burn; gain 2 Block each, cap 35. | 3 each, cap 50. |
| Read Every Blade | 1 Skill, Exhaust | Draw 3; gain 3 Counter per attacking enemy. | Gain 4; no Exhaust. |
| Unfading Guard | 2 Power | Counter loses only 4 at cleanup. | Loses 2. |
| Banked Inferno | 2 Power | Burn loses one third instead of half. | Loses one quarter. |
| Hall of Mirrors | 2 Power | Counter splashes half damage to other enemies. | Splash full damage. |
| Persistent Blaze | 2 Power | Start turn: apply 3 Burn to all. | Apply 5. |
| Ashen Triumph | 1 Power | First Burn kill each turn: +1 Energy, draw 2. | First 2 kills. |
| Heat in the Wound | 2 Power | Counter gains +1 per 4 Burn on attacker. | +1 per 3. |
| Pain into Plate | 2 Power | First unblocked hit each enemy turn grants equal Block, cap 18. | Cap 30. |
| Afterburn | 3 Power | After burned enemy attacks, trigger half remaining Burn. | Trigger full Burn. |
| Paired Resolve | 2 Power | Each Burn-applying card grants 2 Counter once. | Grant 3. |

## Ancient — 2

| Card | Cost / Type | Base | Upgrade |
|---|---|---|---|
| Red Eclipse | 2 Attack, Exhaust | Deal 18 to all; trigger all Burn; gain 5 Counter per enemy damaged. | 24 damage; 7 Counter each. |
| Belder's Last Stand | 2 Skill, Exhaust | Gain 24 Block, 18 Counter; double each enemy Burn, adding at most 20. | 32 Block; 24 Counter; cap 30. |

## Card Art Contract

- Runtime portrait filenames match the unprefixed card ID in snake_case.
- Attack, Skill, and Power portraits use the red, blue, and plum/gold visual languages in `docs/design/ART_DIRECTION.md`.
- Every unique card has a dedicated opaque portrait at `1000x760` and `250x190`; the base and upgraded versions share it.
- Full-size generation sources are retained with the project assets; the reproducible 84-card prompt registry is in `docs/design/cards/ELESIS_CARD_ART_PROMPTS.md`.
