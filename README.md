# Elesis

Playable Slay the Spire 2 character mod for Elesis, based on her base Elsword identity and adapted to a darker STS2-style direction.

## Setup

1. Install or locate Slay the Spire 2 and BaseLib. The current manifest follows the live character template and requires STS2 `0.107.0` or newer plus BaseLib `3.3.8` or newer.
2. Set `STS2_DIR` or `STS2_DATA_DIR` if auto-discovery in `Sts2PathDiscovery.props` does not find the game.
3. Build with:

```sh
scripts/build.sh
```

On the shared VPS setup, use the shared .NET install with a separate CLI home:

```sh
DOTNET_CLI_HOME=/mnt/HC_Volume_105232828/shared/cache/david-dotnet-cli \
DOTNET_ROOT=/mnt/HC_Volume_105232828/shared/tools/dotnet \
PATH="/mnt/HC_Volume_105232828/shared/tools/dotnet:$PATH" \
scripts/build.sh
```

4. Package with:

```sh
scripts/package.sh
```

If the mod uses `.tscn` scenes or Godot-only assets, `scripts/package.sh` automatically switches to Godot export. On the shared VPS setup, pass the local Godot binary:

```sh
GODOT_BIN=/mnt/HC_Volume_105232828/shared/tools/godot/godot-4.5.1/Godot_v4.5.1-stable_mono_linux_x86_64/Godot_v4.5.1-stable_mono_linux.x86_64 \
scripts/package.sh
```

## Local Empire Sword Combat Model

The optional animated combat model is generated locally from the `TDA Empire Sword` BowlRoll archive. Its bundled rules allow editing and non-commercial use but prohibit redistributing an edited model. Consequently, the source PMX, converted GLB, extracted textures, Blender file, and generated Godot imports are ignored by Git. Do not publish a PCK or release ZIP containing this model without permission from the model authors.

Confirmed credits from the archive: EUthanaP / EUthana Project, KOG, TDA, YM, Whine_omo, NIN, XueFei, EUthana_EVE, and DCT丶美玲.

Prerequisites:

- Blender `4.5.12` LTS or another version compatible with the converter.
- [MMD Tools](https://github.com/MMD-Blender/blender_mmd_tools) `v4.5.13` for Blender 4.2+ as a downloaded release ZIP.
- A legitimately obtained `Empire_Sword.zip` archive.

Generate the local Godot model with:

```sh
BLENDER_BIN=/path/to/blender \
scripts/prepare-empire-sword-model.sh \
  /path/to/Empire_Sword.zip \
  /path/to/mmd_tools-v4.5.13-bl4.2.zip
```

The script shows every command, asks before replacing existing local generated files, extracts both PMX models and their textures under `.local/`, attaches the Claymore to Elesis' right wrist, creates `idle` and `attack` skeletal clips, and writes:

```text
Elesis/assets/third_party/empire_sword_local/empire_sword.glb
```

Godot imports that local GLB during the normal package flow. When it exists, every Elesis combat scene renders it through a transparent 3D `SubViewport`; Strike plays its `attack` clip. When it is absent, the existing animated 2D sprite remains the fallback. A package containing the local GLB is suitable for personal testing only under the archive's current rules.

## Project Notes

- Runtime assets live under `Elesis/`.
- C# mod code lives under `ElesisCode/`.
- The mod manifest is `Elesis.json`.
- The BaseLib dependency in `Elesis.json` is versioned with `min_version`; the build updates it from the resolved `Alchyr.Sts2.BaseLib` NuGet version when Godot's restore assets are present.
- The implemented 88-card catalog, including every upgrade, is documented in `docs/design/cards/ELESIS_CARD_POOL.md`.
- The shared art language and reproducible 84-image prompt registry are documented in `docs/design/cards/ELESIS_CARD_ART_PROMPTS.md`.
- Confirmed design decisions are tracked in `docs/`.
