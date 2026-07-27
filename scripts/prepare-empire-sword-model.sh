#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
archive="${1:-}"
mmd_tools_zip="${2:-}"
blender_bin="${BLENDER_BIN:-/mnt/HC_Volume_105232828/shared/tools/blender/blender-4.5.12-linux-x64/blender}"
runtime_lib_dir="${BLENDER_RUNTIME_LIB_DIR:-$repo_root/.local/blender/runtime-libs}"
source_dir="$repo_root/.local/empire_sword/source"
addon_dir="$repo_root/.local/blender/addons/mmd_tools"
output_dir="$repo_root/Elesis/assets/third_party/empire_sword_local"
output_glb="$output_dir/empire_sword.glb"
output_blend="$repo_root/.local/empire_sword/empire_sword.blend"

usage() {
  echo "Usage: scripts/prepare-empire-sword-model.sh /path/to/Empire_Sword.zip /path/to/mmd_tools-v4.5.13-bl4.2.zip" >&2
}

show_and_run() {
  printf '+'
  printf ' %q' "$@"
  printf '\n'
  "$@"
}

if [[ -z "$archive" || -z "$mmd_tools_zip" ]]; then
  usage
  exit 2
fi

if [[ ! -f "$archive" ]]; then
  echo "Empire Sword archive not found: $archive" >&2
  exit 1
fi

if [[ ! -f "$mmd_tools_zip" ]]; then
  echo "MMD Tools archive not found: $mmd_tools_zip" >&2
  exit 1
fi

if [[ ! -x "$blender_bin" ]]; then
  echo "Blender is not executable: $blender_bin" >&2
  exit 1
fi

if ! command -v busybox >/dev/null 2>&1; then
  echo "busybox is required to extract the ZIP archives." >&2
  exit 1
fi

if [[ -e "$output_glb" || -d "$source_dir" ]]; then
  echo "This will overwrite generated local model files under:" >&2
  echo "  $source_dir" >&2
  echo "  $output_dir" >&2
  if [[ "${ELESIS_MODEL_ASSUME_YES:-0}" != "1" ]]; then
    read -r -p "Continue? [y/N] " answer
    if [[ "$answer" != "y" && "$answer" != "Y" ]]; then
      echo "Cancelled."
      exit 0
    fi
  fi
fi

show_and_run mkdir -p "$source_dir" "$addon_dir" "$output_dir"
show_and_run busybox unzip -o "$archive" -d "$source_dir"
show_and_run busybox unzip -o "$mmd_tools_zip" -d "$addon_dir"

opencc_wheel="$(find "$addon_dir/wheels" -maxdepth 1 -type f -name 'opencc*.whl' -print -quit)"
if [[ -z "$opencc_wheel" ]]; then
  echo "The MMD Tools archive does not contain its OpenCC wheel." >&2
  exit 1
fi
show_and_run mkdir -p "$addon_dir/wheels/opencc_unpacked"
show_and_run busybox unzip -o "$opencc_wheel" -d "$addon_dir/wheels/opencc_unpacked"

show_and_run mkdir -p "$source_dir/tex" "$source_dir/t_tex"
show_and_run cp "$source_dir/body/h_face_r2.png" "$source_dir/tex/h_face_r2.png"
show_and_run cp "$addon_dir/externals/MikuMikuDance/toon05.bmp" "$source_dir/t_tex/1s_toon05.bmp"

blender_command=(
  "$blender_bin"
  --background
  --factory-startup
  --python "$repo_root/scripts/convert-empire-sword.py"
  --
  --source-dir "$source_dir"
  --addon-dir "$addon_dir"
  --output-glb "$output_glb"
  --output-blend "$output_blend"
)

if [[ -d "$runtime_lib_dir" ]]; then
  show_and_run env "LD_LIBRARY_PATH=$runtime_lib_dir${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}" "${blender_command[@]}"
else
  show_and_run "${blender_command[@]}"
fi

echo "Generated local Godot model: $output_glb"
echo "The source and generated model remain ignored by Git under the model's no-redistribution terms."
