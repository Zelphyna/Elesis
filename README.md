# Elesis

Playable Slay the Spire 2 character mod for Elesis, based on her base Elsword identity and adapted to a darker STS2-style direction.

## Setup

1. Install or locate Slay the Spire 2 and BaseLib.
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

If the mod uses `.tscn` scenes or Godot-only assets, package with Godot export:

```sh
GODOT_BIN=/path/to/Godot_v4.5.1-stable_mono scripts/package.sh 0.1.0
```

## Project Notes

- Runtime assets live under `Elesis/`.
- C# mod code lives under `ElesisCode/`.
- The mod manifest is `Elesis.json`.
- Confirmed design decisions are tracked in `docs/`.
