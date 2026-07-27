# Project Spec

This repository is a Slay the Spire 2 playable character mod for Elesis.

Confirmed scope:

- Character: Elesis.
- Source identity: base, unevolved Elesis from Elsword.
- Target game: Slay the Spire 2.
- Implementation base: copied STS2 character template using BaseLib.
- Complete card surface: 88 cards — 4 Basic, 20 Common, 36 Uncommon, 26 Rare, and 2 Ancient.
- Draftable pool: 82 cards — 29 Attacks, 35 Skills, and 18 Powers.
- Core card mechanics are Counter Attack and Burn. Counter Attack returns its amount as fixed, blockable, unpowered damage once per received enemy attack hit and normally expires at Block cleanup. Burn deals unblockable, unpowered damage before its monster acts and then decays.
- The 18 durable card Powers build distinct engines around intent reading, Block, Counter Attack, Burn application, Burn decay, retaliation, and conversion between the two mechanics.
- Every card has an implemented upgrade. A card's normal and upgraded versions share one portrait.

Open decisions:

- Final numerical balance after in-game playtesting.
- Future relic and potion expansion beyond the starter surface.
