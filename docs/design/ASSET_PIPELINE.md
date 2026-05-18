# Asset Pipeline

Generated or hand-authored art should be archived with enough context to reproduce or revise it.

Suggested archive structure:

- `docs/design/art_archive/<area>/<asset>/<attempt>/prompt.md`
- `docs/design/art_archive/<area>/<asset>/<attempt>/result.png`

Runtime assets belong under `Elesis/images/`.

Runtime character-form assets must be organized by path and step. Use a `base/` folder for Elesis without specialization. For specialization paths, create one top-level folder per path and place each evolution step inside a numbered folder: `saber_knight_path/1_saber_knight/`, `saber_knight_path/2_grand_master/`, `saber_knight_path/3_empire_sword/`, and the same pattern for Pyro Knight, Dark Knight, and Soar Knight paths. Apply the same folder rule for each scene context that has form-specific scenes, such as `creature_visuals/specializations/<path>/<step_form>/`, `merchant/specializations/<path>/<step_form>/`, and `rest_site/specializations/<path>/<step_form>/`.

Generated image candidates that are not final runtime assets are stored under `Elesis/images/versions/` with context-specific iteration folders, such as `rest-site/base-iteration-01/`. Move selected final assets from there into the specific runtime folder only when the operator asks to integrate or replace an in-game image.

For generated transparent PNGs, the final asset must not keep a visible chroma-key outline or colored fringe. If a temporary chroma-key background is used, validate the result after background removal and reprocess or regenerate the image when a green, magenta, cyan, or other colored contour remains visible. Prefer edge contraction plus color despill on the extracted subject before accepting the asset.
