#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
version="${1:-"$("$repo_root/scripts/mod-version.sh")"}"
mods_dir="${STS2_MODS_DIR:-/mnt/HC_Volume_105232828/shared/games/slay-the-spire-2/mods}"
mod_id="${ELESIS_MOD_ID:-Elesis}"
mod_dir="$mods_dir/$mod_id"
dist_dir="$repo_root/dist"
asset="$dist_dir/$mod_id-$version.zip"

mkdir -p "$dist_dir"

"$repo_root/scripts/build.sh"

if [[ -n "${ELESIS_PCK_EXPORTER:-}" ]]; then
  pck_exporter="$ELESIS_PCK_EXPORTER"
elif find "$repo_root/$mod_id" -type f -name '*.tscn' -print -quit | grep -q .; then
  pck_exporter="godot"
else
  pck_exporter="quick"
fi

case "$pck_exporter" in
  quick)
    ;;
  godot)
    "$repo_root/scripts/export-pck-godot.sh" "$mod_dir/$mod_id.pck"
    ;;
  *)
    echo "Unknown ELESIS_PCK_EXPORTER value: $pck_exporter" >&2
    echo "Expected 'quick' or 'godot'." >&2
    exit 1
    ;;
esac

for file in "$mod_id.dll" "$mod_id.pck" "$mod_id.json"; do
  if [[ ! -f "$mod_dir/$file" ]]; then
    echo "Missing build output: $mod_dir/$file" >&2
    exit 1
  fi
done

python3 - "$asset" "$mod_dir" "$mod_id" <<'PY'
import pathlib
import sys
import zipfile

asset = pathlib.Path(sys.argv[1])
mod_dir = pathlib.Path(sys.argv[2])
mod_id = sys.argv[3]
files = [f"{mod_id}.dll", f"{mod_id}.pck", f"{mod_id}.json"]

with zipfile.ZipFile(asset, "w", zipfile.ZIP_DEFLATED) as archive:
    for name in files:
        archive.write(mod_dir / name, f"{mod_id}/{name}")

print(asset)
PY
