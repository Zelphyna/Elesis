# Architecture

## Current Structure

- `Elesis.json`: STS2 mod manifest.
- `Elesis.csproj`: C# project and BaseLib dependency setup.
- `Elesis/`: Godot resources, localization, and runtime assets included in the `.pck`.
- `ElesisCode/`: C# mod code.
- `ElesisCode/MainFile.cs`: mod initializer; registers the custom Elesis character with BaseLib.
- `ElesisCode/Character/Elesis.cs`: custom character model. Elesis intentionally extends `CustomCharacterModel`, matching the working Hologirl mod pattern; `PlaceholderCharacterModel` retained vanilla fallback icon behavior and made character selection unreliable. Elesis uses the same safe Ironclad audio, transition, visual, trail, and energy-counter fallbacks as Hologirl until custom STS2-safe assets/events exist.
- `ElesisCode/Relics/BelderKnightEmblem.cs`: starter relic used so character selection and run start have an Elesis-owned relic model to display and equip.
- `scripts/`: build, package, and release helpers.

## Build Flow

- `scripts/build.sh` builds the C# project and copies DLL/JSON outputs to the local STS2 mods folder.
- `scripts/package.sh` creates `dist/Elesis-<version>.zip`.
- `scripts/package.sh` automatically uses Godot export when `Elesis/` contains `.tscn` scenes, because the quick PCK packer skips those files.
- Use `GODOT_BIN=/path/to/godot` when Godot export is selected. `ELESIS_PCK_EXPORTER=quick|godot` remains available for explicit overrides.

Update this file when the codebase architecture changes.
