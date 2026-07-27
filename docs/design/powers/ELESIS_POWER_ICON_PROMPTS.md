# Elesis Power icon prompts

These prompts define the dedicated combat-status emblems generated for the
Elesis card pool. Runtime assets use a 64×64 packed icon and a 256×256 large
icon with transparent padding. The source generation used the built-in image
generator, followed by chroma-key removal and alpha-safe resizing.

## Shared art direction

> One centered symbolic power emblem, no character portrait, bold silhouette
> readable at 64×64. Clean modern anime game UI emblem matching Elesis: crisp
> cel shading, thick dark outline, crimson `#A32638`, silver, black inner
> shadows, warm gold accents. Flat solid `#00ff00` chroma-key background only,
> no cast shadow or glow on the green, background texture, text, letters, logo,
> watermark, outer frame, or green in the subject. Square 1024×1024.

Each generated prompt prepended the visual concept below to the shared art
direction.

| Power | Visual concept |
| --- | --- |
| Ever Ready | Poised silver sword and ready crimson spark crest |
| Cinder Etching | Glowing ember rune etched into a silver blade |
| Tempered Retort | Hammered shield returning a heated sword strike |
| Aegis Teeth | Toothed silver shield with a crimson counter edge |
| Ashen Shelter | Protective silver canopy sheltering red embers |
| Punishing Rhythm | Repeating impact arcs around a counter blade |
| Deep Scorch | Deep blade scar filled with concentrated flame |
| Falling Embers | Descending ember cluster splitting toward targets |
| Furnace Rampart | Fortified silver wall enclosing a furnace core |
| Unfading Guard | Enduring shield around an undying red spark |
| Banked Inferno | Sealed furnace storing a compressed inferno |
| Hall of Mirrors | Mirrored counter blades reflecting one impact |
| Persistent Blaze | Steady multi-layer flame that refuses to fade |
| Ashen Triumph | Victorious gold-edged crest rising from ashes |
| Heat in the Wound | Burning wound mark feeding a counter blade |
| Pain into Plate | Blood-red impact transforming into silver armor |
| Afterburn | Fast silver sword trailing a curling crimson flame |
| Paired Resolve | Linked counter-sword and flame crests |
| Reprisal Cut | Returning crimson slash crossing a counter blade |
| Answered Attack | Faded sword-impact echo behind a counter crest |
| Burn Stabilizer | Iron clamp ring slowing a contained flame |
| Sealed Counter | Counter sword locked by a crimson-and-gold seal |
| Double Counter | Two mirrored counter blades striking in sequence |
| Counter Retention | Armored gauntlet preserving a counter spark |
| Redirection | Curved silver arrow deflecting a blade into flame |
| Smoldering Guard | Cracked shield leaking red embers |
| Aggressor Brand | Dark target branded by attacks and an ember scar |

`Answered Attack` remains a hidden bookkeeping state. `Sealed Counter` is a
