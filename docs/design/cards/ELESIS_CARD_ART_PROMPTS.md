# Elesis Card Art Prompt Set

Generation pass: 2026-07-26. Mode: built-in Codex ImageGen, one independent bitmap generation call per new card. No contact sheets were used.

The four existing starter portraits (`elesis_strike`, `elesis_defend`, `counter_guard`, and `burning_edge`) were retained as the style anchors. The 84 new cards use the prompt system below.

## Shared Prompt

> Horizontal card portrait, 25:19, opaque background, no card frame, no border, no text, no numbers, no UI, and no logo. Clean modern anime game art, crisp cel shading, visible dark outlines, moderate controlled detail, one strong focal subject, two or three large value masses, and a silhouette readable at thumbnail size. When Elesis appears, show only base-form Elesis: a young heroic swordswoman with a deep crimson high ponytail, red eyes, red, black, and silver knight armor with warm-gold and white accents, and an oversized red-and-white sword. Do not show an advanced specialization. Avoid chibi, photorealism, painterly grime, excessive glow, a static character bust, or cluttered splash-art staging.

Type addenda:

- **Attack:** genuinely crimson/red background, forceful diagonal composition, restrained orange sparks, and a clear direction of impact.
- **Skill:** genuinely cool-blue background, stable, circular, defensive, or technical composition, with crimson/silver/orange used only as accents.
- **Power:** deep plum/violet background with warm-gold and controlled-crimson accents; centered, stable, emblematic composition rather than a single attack moment.
- **Ancient:** preserve the card's Attack or Skill language, then add a restrained red-gold-white convergence effect with a larger mythic silhouette.

## Subject Registry

The subject directive in this table is appended to the shared prompt and the matching type addendum.

| Type | Card ID | Subject directive |
|---|---|---|
| Attack | `measured_slash` | Elesis reads a raised enemy weapon and answers with one precise diagonal cut. |
| Attack | `ember_thrust` | Elesis drives the ember-hot sword point through a dark enemy guard. |
| Attack | `scorching_feint` | Boots and blade make a deceptive sidestep around a burned silhouette. |
| Attack | `guarded_cut` | A gauntlet and broad sword catch an incoming weapon while cutting past it. |
| Attack | `kindled_sweep` | A wide sword arc ignites several enemy silhouettes. |
| Attack | `answering_blow` | A crossed-blade impact rebounds as a sharp returning slash. |
| Attack | `coalbrand` | The red-white sword heats a new rune over black forge coals. |
| Attack | `opening_read` | One enemy intent branches into a counter path or a Burn brand. |
| Attack | `searing_pommel` | The ornate pommel strikes an existing Burn mark on enemy armor. |
| Attack | `crossfire_slash` | Two sword trails form an X and ignite a Burn sigil at the crossing. |
| Attack | `challenge_sweep` | Elesis sweeps several enemies while a counter crest rises behind her. |
| Skill | `ready_guard` | Elesis plants her sword in a calm guard inside a shield-and-counter halo. |
| Skill | `heat_haze` | Heat distortion passes over three enemies and brands each with an ember. |
| Skill | `low_guard` | Armored boots and a low-held sword brace beneath looming enemy weapons. |
| Skill | `ashen_veil` | A blue ash veil thickens around armor when it sees a burned enemy. |
| Skill | `red_challenge` | A knight pennant marks one weakened aggressor beside a counter seal. |
| Skill | `banked_spark` | One ember is sealed inside a silver-and-crimson knight reliquary. |
| Skill | `steady_breath` | Elesis breathes inside a calm blue ring as counter sparks settle on her blade. |
| Skill | `covering_embers` | A blue shield shelters the foreground while embers spill onto distant enemies. |
| Skill | `rooted_defense` | Boots and a sword planted in stone form an immovable defensive anchor. |
| Attack | `reprisal_cut` | Elesis retaliates immediately after a counter impact. |
| Attack | `burning_reversal` | Elesis turns an incoming strike back as a flaming reversal. |
| Attack | `cinder_barrage` | Three successive cuts leave three distinct cinder wounds. |
| Attack | `pressure_break` | The oversized sword breaks a heavy shield under stored pressure. |
| Attack | `flashpoint` | A sword point precisely detonates a Burn mark without erasing its rings. |
| Attack | `backstep_cleave` | Elesis retreats while cleaving across several enemies. |
| Attack | `tempered_edge` | A sword edge heats in visible stages on a forge anvil. |
| Attack | `provoked_assault` | Multiple incoming weapons build a counter emblem around one guard. |
| Attack | `charred_wound` | A new cut reopens an older glowing scar. |
| Attack | `twin_verdict` | Two crossed verdict strikes resolve as Burn first and Counter second. |
| Skill | `layered_defense` | Elesis stands behind three offset defensive planes. |
| Skill | `countermeasure` | Gauntlets and sword lock an incoming weapon in a precise mechanism. |
| Skill | `shelter_in_sparks` | Sword and shield form a triangular refuge in a shower of sparks. |
| Skill | `fan_the_ashes` | A blue wind arc distributes cinders toward several enemies. |
| Skill | `transfer_heat` | Heat leaves burned armor and fills a crossed-blade counter crest. |
| Skill | `redirection` | Shield and sword bend a spear trajectory back toward its owner. |
| Skill | `calculated_risk` | Elesis accepts a small wound to form a much larger counter seal. |
| Skill | `cinder_screen` | A cobalt ash curtain protects a sword, shield, and breastplate. |
| Skill | `burning_patience` | Elesis waits behind a stable guard with contained embers. |
| Skill | `steel_nerves` | A still breastplate holds firm under repeated weapon impacts. |
| Skill | `echo_guard` | Two concentric defensive echoes extend one guard. |
| Skill | `ash_reclamation` | Consumed ash is gathered into a knight reliquary. |
| Skill | `shared_threat` | A burned enemy mask and counter shield share one warning tether. |
| Skill | `watch_the_blade` | Elesis studies several incoming blades at once. |
| Skill | `smoldering_guard` | A cracked shield sends its embers back into an attacking claw. |
| Skill | `delay_the_strike` | An enemy sword hangs suspended in blue timing ribbons. |
| Skill | `tactical_withdrawal` | Elesis retreats while preserving a counter emblem under cover. |
| Power | `ever_ready` | Elesis stands sentinel beside a vertical sword and counter crest. |
| Power | `cinder_etching` | A cinder rune is engraved into a steel disk. |
| Power | `tempered_retort` | Crossed swords become a crown of controlled embers. |
| Power | `aegis_teeth` | A silver shield bears a toothed counter edge. |
| Power | `ashen_shelter` | A brazier burns safely beneath a violet protective dome. |
| Power | `punishing_rhythm` | Elesis stands inside regular concentric counter pulses. |
| Power | `deep_scorch` | One incandescent cut passes through several steel layers. |
| Power | `falling_embers` | An emblem-tree moves embers between enemy masks. |
| Power | `furnace_rampart` | Elesis faces a shield-wall containing a furnace heart. |
| Attack | `crimson_reprisal` | A decisive retaliation slash repeats as two echo arcs. |
| Attack | `pyre_divide` | The blade splits a Burn sigil into immediate fire while its rings remain. |
| Attack | `perfect_answer` | Half of a flawless strike folds back into a counter crest. |
| Attack | `scarlet_crossfire` | Three impacts surround one enemy and each leaves a Burn brand. |
| Attack | `sentence_of_ash` | A judicial sword condemns a row of burned enemies to ash. |
| Attack | `mirrorsteel_lunge` | A mirror-bright lunge reflects two waiting counter impacts. |
| Attack | `lasting_scar` | A severe new scar doubles the Burn pattern across enemy armor. |
| Attack | `red_horizon` | A crimson horizon damages all, burns all, and raises a counter crest. |
| Skill | `absolute_guard` | Elesis and her vertical sword anchor a massive shield-counter halo. |
| Skill | `sealed_defense` | A sword and counter spark remain preserved inside a luminous seal. |
| Skill | `ash_cascade` | A blue cascade distributes ash and then releases stored heat across enemies. |
| Skill | `borrowed_heat` | Heat leaves an enemy brand and hardens into blue armor plates. |
| Skill | `return_to_sender` | Several intent arrows strike a guard and return as equal block-counter segments. |
| Skill | `brand_the_aggressor` | A branded enemy weapon burns hotter with every outgoing hit. |
| Skill | `encircled_bulwark` | A central bulwark gains layers from several surrounding attackers. |
| Skill | `rekindled_defense` | Burn marks drain from enemies and rebuild a shattered shield. |
| Skill | `read_every_blade` | A tactical overhead view resolves every incoming weapon path. |
| Power | `unfading_guard` | A counter crest remains lit while outer cleanup rings fade. |
| Power | `banked_inferno` | A furnace stores its fire behind four measured decay gates. |
| Power | `hall_of_mirrors` | A central counter blade reflects damage through a hall of enemy mirrors. |
| Power | `persistent_blaze` | A perpetual ember crown feeds several enemy brands. |
| Power | `ashen_triumph` | Fallen ash masks release energy and cards into a gold-red crest. |
| Power | `heat_in_the_wound` | Burn marks along an enemy weapon sharpen a counter sword. |
| Power | `pain_into_plate` | A received impact is forged immediately into a new armor plate. |
| Power | `afterburn` | An enemy finishes a swing and its remaining brand flares afterward. |
| Power | `paired_resolve` | A Burn seal and counter crest orbit as a stable matched pair. |
| Ancient Attack | `red_eclipse` | A red-gold eclipse joins a battlefield-wide slash, all Burn, and many counters. |
| Ancient Skill | `belders_last_stand` | Belder's sword-banner anchors a final shield while every enemy brand doubles. |

## Saved Outputs

For every new `<id>` in the registry:

```text
Elesis/images/card_portraits/source/generated-2026-07-26/<id>_source.png
Elesis/images/card_portraits/big/<id>.png        # 1000x760
Elesis/images/card_portraits/<id>.png            # 250x190
```

The runtime image is shared by the normal and upgraded card. The source files remain available for later recropping or a controlled regeneration pass.
