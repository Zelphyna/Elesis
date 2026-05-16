#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 2 || $# -gt 3 ]]; then
  echo "Usage: scripts/archive-art-attempt.sh <archive-key> <source-png> [prompt-file]" >&2
  echo "Examples:" >&2
  echo "  scripts/archive-art-attempt.sh livestream /path/generated.png /tmp/prompt.md" >&2
  echo "  scripts/archive-art-attempt.sh menu/character_select_splash /path/generated.png /tmp/prompt.md" >&2
  exit 1
fi

archive_key="$1"
source_png="$2"
prompt_file="${3:-}"
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ "$archive_key" == */* ]]; then
  archive_root="$repo_root/docs/design/art_archive/$archive_key"
else
  archive_root="$repo_root/docs/design/art_archive/cards/$archive_key"
fi

if [[ ! -f "$source_png" ]]; then
  echo "Source image not found: $source_png" >&2
  exit 1
fi

mkdir -p "$archive_root"

next_number="$(
  find "$archive_root" -maxdepth 1 -type d -name 'attempt-*' -printf '%f\n' \
    | sed 's/^attempt-//' \
    | sort -n \
    | tail -1
)"

if [[ -z "$next_number" ]]; then
  next_number="1"
else
  next_number="$((10#$next_number + 1))"
fi

attempt_id="$(printf 'attempt-%03d' "$next_number")"
attempt_dir="$archive_root/$attempt_id"
mkdir -p "$attempt_dir"

cp "$source_png" "$attempt_dir/result.png"

if [[ -n "$prompt_file" ]]; then
  cp "$prompt_file" "$attempt_dir/prompt.md"
else
  cat > "$attempt_dir/prompt.md" <<'EOF'
# Prompt

Exact prompt not captured.
EOF
fi

generated_at="$(stat -c '%y' "$source_png" | sed 's/ +0000$//')"
source_name="$(basename "$source_png")"
title_name="$(printf '%s' "$archive_key" | sed -E 's#[/_-]+# #g; s/(^| )([a-z])/\U\2/g')"

cat > "$attempt_dir/README.md" <<EOF
# $title_name ${attempt_id^}

- Generated: $generated_at UTC
- Source: \`$source_name\`
- Result: \`result.png\`
- Prompt: \`prompt.md\`
- Prompt status: exact if \`prompt.md\` was provided; otherwise missing.
- Selection status: undecided.

## Notes

Fill this in after visual review.
EOF

echo "$attempt_dir"
