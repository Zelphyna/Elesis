# Asset Pipeline

Generated or hand-authored art should be archived with enough context to reproduce or revise it.

Suggested archive structure:

- `docs/design/art_archive/<area>/<asset>/<attempt>/prompt.md`
- `docs/design/art_archive/<area>/<asset>/<attempt>/result.png`

Runtime assets belong under `Elesis/images/`.

Generated image candidates that are not final runtime assets are stored under `Elesis/images/versions/` with versioned filenames. Move selected final assets from there into the specific runtime folder when they are integrated.

For generated transparent PNGs, the final asset must not keep a visible chroma-key outline or colored fringe. If a temporary chroma-key background is used, validate the result after background removal and reprocess or regenerate the image when a green, magenta, cyan, or other colored contour remains visible. Prefer edge contraction plus color despill on the extracted subject before accepting the asset.
