# Development Steps

Last updated: 2026-07-07.

## Current Confirmed State

- [x] Rename the template identifiers to the real mod id. `Elesis.json` and `MainFile.ModId` both use `Elesis`.
- [x] Keep the manifest aligned with the current known STS2/BaseLib dependency shape. The manifest targets STS2 `0.107.0` or newer and BaseLib `3.3.5` or newer.
- [x] Define the character's gameplay identity. The confirmed Counter Attack direction lives in `docs/design/GAMEPLAY_IDENTITY.md`.
- [x] Implement the first playable prototype surface: custom character model, starter relic, prototype card pool, visible powers, combat XP reward, specialization events, evolution events, localization, scenes, and packaged assets.
- [x] Replace the initial placeholder character surface with Elesis-specific runtime assets for character select, combat, merchant, rest site, relics, powers, events, and card portraits.

## Next Validation Pass

- [ ] Run `scripts/build.sh` on the current machine and fix any compile or local install issue.
- [ ] Run `scripts/package.sh` with the Godot exporter path when needed and confirm the release zip is produced.
- [ ] Load Elesis in-game and verify character selection, starter deck, starter relic, energy counter, combat visuals, merchant visuals, rest-site visuals, and localization.
- [ ] Play through the specialization flow in-game: claim combat XP rewards, trigger the 15 XP branch choice, trigger the 35 XP evolution, and trigger the 55 XP final evolution.
- [ ] Check that the chosen specialization updates combat, merchant, and rest-site visuals without duplicate events or unsafe map transitions.

## Proposed Next Work

- [ ] Balance pass 1: test the starter deck and first-act feel, then adjust Counter Attack gain, Burn application, and starter relic values.
- [ ] Mechanics pass: verify Counter Attack triggers once per received attack hit and only when an enemy attacks Elesis.
- [ ] Mechanics pass: verify Burn triggers before enemy attacks, deals its full amount, then halves with rounded-up loss.
- [ ] Item/relic direction pass: decide whether "items" means relics, potions, Ancient cards, event rewards, or all of them before adding mechanics beyond `BelderKnightEmblem`.
- [ ] Card pool expansion: move toward a full STS2-style pool by adding mostly Skills and Powers, as tracked in `docs/design/cards/ELESIS_CARD_POOL.md`.
- [ ] Counter Attack definition: decide whether the counter amount should persist for the whole combat or decay at turn end after playtesting.
- [ ] Art polish pass: replace any remaining prototype card art, normalize portrait framing, and archive rejected art attempts when useful.
- [ ] Release hygiene: after a successful build, package, and playtest pass, update release notes for the next version.
