# Elesis Card Pool

Current implemented card pool: 4 cards.

Current implementation status:

- The previous prototype card pool has been removed.
- `CounterAttackPower` is the implemented Elesis class mechanic power.
- `BurnPower` is an enemy debuff. Before the monster acts, it takes Burn damage, then loses half of its Burn, rounded up.
- `ElesisStrike` is a standard 1-cost Basic Attack: 6 damage, upgrade +3.
- `ElesisDefend` is a standard 1-cost Basic Skill: 5 Block, upgrade +3.
- `CounterGuard` is a 1-cost Basic Skill: gain 6 Counter Attack, upgrade +3.
- `BurningEdge` is a 1-cost Basic Skill: apply 5 Burn to an enemy, upgrade +3.
- Starting deck is 10 cards: 4 `ElesisStrike`, 4 `ElesisDefend`, 1 `CounterGuard`, and 1 `BurningEdge`.
- The current cards have dedicated generated card portraits.

## Card Art Rules

- Attack card images must use a red background as the actual art background, not as a contour or frame.
- Skill card images must use a blue background as the actual art background, not as a contour or frame.
- Keep Elesis card art simple, readable, and aligned with the base Elesis art direction: clean anime game-art, crisp cel-shading, visible dark outlines, controlled crimson, black support tones, light silver armor, and restrained warm-gold accents.

## Design Target

- Confirm the Counter Attack and Burn hooks in-game before expanding the card pool.
- Add future cards only when they support Counter Attack, Burn, or their interaction.
- Keep early cards simple enough to test single-hit, multi-hit, non-attacking enemy turns, and Burn decay.
