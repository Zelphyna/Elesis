# Gameplay Identity

Use this file for the character's confirmed mechanical identity, archetypes, and constraints.

## Confirmed Current Identity

- Elesis' class mechanic is Counter Attack.
- Counter Attack is a visible counter power. When Elesis is attacked, she deals Counter Attack damage back to the attacker once for each hit received.
- Multi-hit enemy attacks trigger Counter Attack once per hit. For example, 6 Counter Attack returns 6 damage against a single 12-damage hit, and 18 total damage against a 3x3 attack.
- Enemies that do not attack do not take Counter Attack damage.
- Burn is a visible enemy debuff. Before the burned monster acts, it takes damage equal to its Burn, then loses half of its Burn, rounded up.
- The current starter deck uses 4 `ElesisStrike`, 4 `ElesisDefend`, 1 `CounterGuard`, and 1 `BurningEdge`.
- `CounterGuard` is a 1-cost Skill that gives 6 Counter Attack and upgrades by 3.
- `BurningEdge` is a 1-cost Skill that applies 5 Burn to an enemy and upgrades by 3.
- Belder Knight Emblem tracks run-local specialization experience. It starts at 0 each run. Combat XP is offered as a selectable post-combat reward: normal combats grant 3 XP, elites grant 4 XP, and bosses grant 6 XP. Non-combat nodes do not grant specialization XP. At 15 XP, once the map is reopened, Elesis chooses a visual specialization path for the rest of the run.

## Next Mechanics Hypothesis

These notes are proposed design direction, not confirmed implementation. They should be validated against playtests and the current STS2/BaseLib API before code changes.

- Decide whether Counter Attack should be retained for the full combat, last for one turn, or decay by a fixed amount at turn end.
- Confirm that Burn timing fires before enemy attacks in the current STS2 turn sequence.
- Build future cards around preparing for enemy attack patterns: efficient Counter Attack gains, Block plus Counter Attack, and payoffs for surviving multi-hit turns.
- Avoid adding unrelated stance, fire, or resource loops until Counter Attack is tested as the primary identity.

## Item And Relic Hypothesis

`Belder Knight Emblem` is currently the specialization XP tracker. Keep that ownership unless a playtest shows the counter is too overloaded.

Proposed item/relic direction:

- Starter relic: keep it focused on progression until the Counter Attack loop is stable.
- Counter Attack relics: reward being attacked, surviving multi-hit turns, or keeping Counter Attack above a threshold.

Open question: "items" may mean relics, potions, Ancient cards, or event rewards. This repository currently has one implemented relic and no Elesis-specific potion behavior, so the next design pass should decide the item scope before implementation.
