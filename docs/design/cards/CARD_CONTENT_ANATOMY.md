# Card Content Anatomy

Use this file as a reference checklist before changing Elesis cards or card
assets. It separates the visible card presentation from the gameplay/model data.

## Visual Content

- Illustration: the main card artwork.
- Card background: the color or texture associated with the character, card
  family, or card type.
- Card base/frame/contour: the structural frame around the illustration and text.
  For Elesis card-frame work, this is the target area when changing the base of a
  card.
- Rarity banner: a separate visual element tied to rarity, such as Common,
  Uncommon, or Rare. Do not treat this as part of the base/frame when the request
  is specifically about the card contour.
- Name area: the displayed card title.
- Cost orb: the energy-cost visual, usually near the upper-left area.
- Type banner or type plate: the visual area that labels the card as an Attack,
  Skill, Power, Curse, Status, or another supported type.
- Description area: the rules text box.
- Inline icons: visual symbols embedded in card text, such as damage, block,
  energy, or custom keyword symbols.
- Upgrade presentation: upgraded cards usually show a `+` on the displayed name
  and may change highlighted values or text.

For STS2-style card asset changes, keep these asset categories conceptually
separate:

- `Frame...`: card frame/base/contour.
- `Bg...`: card background behind the main card content.
- `Type...`: card type plate or type treatment.
- `Banner...`: rarity banner; avoid this when only changing the card base.
- `Card...Orb`: character or energy-cost orb treatment.

## Gameplay Content

- Internal name or ID: the code/model identifier used by the game or mod. This can
  differ from the displayed card name.
- Display name: the localized name shown to the player.
- Cost: the energy cost, such as `0`, `1`, `2`, `X`, or a dynamically modified
  value.
- Type: Attack, Skill, Power, Status, Curse, or another supported model type.
- Rarity: Basic, Common, Uncommon, Rare, Special, or another supported rarity.
- Character or pool ownership: the character, neutral pool, event pool, or custom
  pool that can provide the card.
- Description: the localized rules text shown on the card.
- Numeric values: damage, block, draw, debuff amount, scaling amount, and other
  model-backed values.
- Keywords: evergreen or custom terms such as Exhaust, Ethereal, Innate, Retain,
  Unplayable, or Elesis-specific keywords.
- Upgrade data: the upgraded version's cost, values, keywords, and description
  changes.
- Special conditions: effects that depend on discard, exhaust, hand state, draw
  pile, enemy state, or other combat conditions.
- Hidden tags and model flags: internal tags used by combat logic, generation,
  removability, temporary cards, quest cards, or other systems.

## Working Rule For Future Card Changes

When modifying a card, first identify whether the request is about visual assets,
gameplay data, or both. If the request says "base", "contour", or "frame", prefer
the card frame/base assets and do not change the rarity banner unless the operator
explicitly asks for that.
