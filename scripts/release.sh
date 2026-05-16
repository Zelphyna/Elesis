#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
version="${1:-"$("$repo_root/scripts/mod-version.sh")"}"
tag="$version"
if [[ "$tag" != v* ]]; then
  tag="v$tag"
fi
mod_id="${ELESIS_MOD_ID:-Elesis}"
asset="$repo_root/dist/$mod_id-$tag.zip"
title="$mod_id $tag"
notes_file="$repo_root/docs/releases/$tag.md"
notes="${2:-""}"

"$repo_root/scripts/package.sh" "$tag"

cd "$repo_root"

if gh release view "$tag" >/dev/null 2>&1; then
  gh release upload "$tag" "$asset" --clobber
  if [[ -f "$notes_file" ]]; then
    gh release edit "$tag" --latest --prerelease=false --title "$title" --notes-file "$notes_file"
  else
    if [[ -z "$notes" ]]; then
      notes="$mod_id prototype $tag."
    fi
    gh release edit "$tag" --latest --prerelease=false --title "$title" --notes "$notes"
  fi
else
  if [[ -f "$notes_file" ]]; then
    gh release create "$tag" "$asset" --latest --title "$title" --notes-file "$notes_file"
  else
    if [[ -z "$notes" ]]; then
      notes="$mod_id prototype $tag."
    fi
    gh release create "$tag" "$asset" --latest --title "$title" --notes "$notes"
  fi
fi
