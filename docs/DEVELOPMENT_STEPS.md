# Development Steps

Last updated: 2026-07-26.

## Current Confirmed State

- [x] Rename the template identifiers to the real mod id. `Elesis.json` and `MainFile.ModId` both use `Elesis`.
- [x] Keep the manifest aligned with the current known STS2/BaseLib dependency shape. The manifest targets STS2 `0.107.0` or newer and BaseLib `3.3.8` or newer.
- [x] Define the character's gameplay identity. The confirmed Counter Attack direction lives in `docs/design/GAMEPLAY_IDENTITY.md`.
- [x] Implement the first playable prototype surface: custom character model, starter relic, prototype card pool, visible powers, combat XP reward, specialization events, evolution events, localization, scenes, and packaged assets.
- [x] Replace the initial placeholder character surface with Elesis-specific runtime assets for character select, combat, merchant, rest site, relics, powers, events, and card portraits.
- [x] Expand the card surface to 88 cards: 4 Basic, 20 Common, 36 Uncommon, 26 Rare, and 2 Ancient.
- [x] Implement a dedicated upgrade for every card and 18 distinct durable card Powers.
- [x] Generate a dedicated type-coded portrait for every card and retain the full-size generation sources.

## Next Validation Pass

- [x] Run `scripts/build.sh` on the current machine. The card-pool build passes with 0 warnings and 0 errors; quick PCK packing remains intentionally skipped because the project contains `.tscn` scenes and must use Godot export.
- [x] Run `scripts/package.sh` with the Godot exporter path and confirm `dist/Elesis-v0.4.62.zip` is produced with the DLL, PCK, and manifest.
- [ ] Load Elesis in-game and verify character selection, starter deck, starter relic, energy counter, combat visuals, merchant visuals, rest-site visuals, and localization.
- [ ] Play through the specialization flow in-game: claim combat XP rewards, trigger the 15 XP branch choice, trigger the 35 XP evolution, and trigger the 55 XP final evolution.
- [ ] Check that the chosen specialization updates combat, merchant, and rest-site visuals without duplicate events or unsafe map transitions.

## Proposed Next Work

- [ ] Balance pass 1: test the starter deck and first-act feel, then adjust Counter Attack gain, Burn application, and starter relic values.
- [ ] Mechanics pass: verify Counter Attack, its retention/amplification Powers, and multi-hit behavior in live combat.
- [ ] Mechanics pass: verify Burn, direct triggers, alternate decay divisors, propagation, and kill rewards in live combat.
- [ ] Item/relic direction pass: decide whether "items" means relics, potions, Ancient cards, event rewards, or all of them before adding mechanics beyond `BelderKnightEmblem`.
- [ ] Counter Attack definition: decide whether the counter amount should persist for the whole combat or decay at turn end after playtesting.
- [ ] Art polish pass: replace any remaining prototype card art, normalize portrait framing, and archive rejected art attempts when useful.
- [ ] Release hygiene: after a successful build, package, and playtest pass, update release notes for the next version.
