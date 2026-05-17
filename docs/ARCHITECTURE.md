# Architecture

## Current Structure

- `Elesis.json`: STS2 mod manifest.
- `Elesis.csproj`: C# project and BaseLib dependency setup.
- `Elesis/`: Godot resources, localization, and runtime assets included in the `.pck`.
- `ElesisCode/`: C# mod code.
- `ElesisCode/MainFile.cs`: mod initializer; registers the custom Elesis character with BaseLib.
- `ElesisCode/Character/Elesis.cs`: custom character model. Elesis intentionally extends `CustomCharacterModel`, matching the working Hologirl mod pattern; `PlaceholderCharacterModel` retained vanilla fallback icon behavior and made character selection unreliable. Elesis uses generated static scenes for combat and rest-site visuals. Ironclad fallbacks remain for audio, transition, trail, merchant animation, and multiplayer hand textures until custom STS2-safe replacements exist. The run/top-panel icon is generated from Elesis' custom icon texture, and the multiplayer map outline uses the Elesis map marker texture.
- `ElesisCode/Cards/ElesisKeywords.cs`: custom BaseLib `CardKeyword` definitions used to show hover-tip boxes for Elesis mechanics on card hover.
- `Elesis/scenes/creature_visuals/`, `Elesis/scenes/combat/`, and `Elesis/scenes/rest_site/`: static scenes used by BaseLib's character scene conversion hooks. Combat visuals must provide a `Visuals` child plus combat anchor nodes so BaseLib can convert the scene to `NCreatureVisuals`. The custom energy counter scene supplies Elesis' large energy orb while BaseLib generates the required label and particle containers.
- `ElesisCode/Relics/BelderKnightEmblem.cs`: starter relic used so character selection and run start have an Elesis-owned relic model to display and equip.
- `ElesisCode/Specializations/`: run-local specialization flow for Elesis. The controller listens for map re-entry after completed nodes, awards Belder Knight Emblem XP, opens the specialization event at 15 XP, opens evolution events at 35 and 55 XP, and the visual helper selects the combat scene for the current branch tier.
- `ElesisCode/Rewards/`: custom reward types. `ElesisExperienceReward` is injected into combat rewards by Belder Knight Emblem so combat XP is claimed from the reward screen; non-combat XP remains awarded on map re-entry.
- `scripts/`: build, package, and release helpers.

## Build Flow

- `scripts/build.sh` builds the C# project and copies DLL/JSON outputs to the local STS2 mods folder.
- `scripts/package.sh` creates `dist/Elesis-<version>.zip`.
- `scripts/package.sh` automatically uses Godot export when `Elesis/` contains `.tscn` scenes, because the quick PCK packer skips those files.
- Use `GODOT_BIN=/path/to/godot` when Godot export is selected. `ELESIS_PCK_EXPORTER=quick|godot` remains available for explicit overrides.

Update this file when the codebase architecture changes.
