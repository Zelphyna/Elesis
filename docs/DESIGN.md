# Design

Confirmed design direction:

- Elesis should read as her base Elsword version, not a Grand Chase variant.
- Avoid advanced Elsword class-path details such as Grand Master, Blazing Heart, Bloody Queen, Empire Sword, or Flame Lord.
- Visual style should be simplified and adapted to Slay the Spire 2: dark fantasy, painterly, readable silhouettes, rough brush texture, and controlled crimson accents.
- Gameplay identity should start from sword-focused offense with restrained fire motifs, not from full evolution-class spectacle.
- The first implemented card pool uses prototype-safe card effects grouped by Vitality, Destruction, Flame, and Parry. Chivalry and Flame are implemented as counter powers; Parry is still a design label for efficient defensive cards.
- Card portraits have a first functional art pass. The 40 implemented cards each have runtime and big portrait PNGs named after their card IDs, with source art grouped by current mechanics and rarity beats.
- Character selection uses a Belder/Velder fortress backdrop with Elesis shown from head to mid-thigh, while the clickable selector icon stays focused on her head and shoulders and follows the template's `132x195` selector image size.
- The starter relic visual is the Belder Knight Emblem: a red-and-gold knight crest with a claymore motif, matching her red knight identity.
- Energy UI uses a red crystal-and-gold knight orb. Chivalry uses a red-gold sword crest power icon, and Flame uses a sharper red-orange sword-flame crest icon.
- Map marker art currently uses the previous larger Elesis marker because it reads better in-game than the experimental silhouette pass.
- Combat, shop, and rest-site character art use transparent sprite assets with bottom-aligned scene placement. Combat art leans more chibi/readable than the larger menu illustrations so her feet align cleanly with other player visuals.
- Card portraits use the combat chibi Elesis as a shared readable character motif. Attack portraits lean red with slash/flame shapes, Skill portraits lean blue with guard/movement shapes, and Power portraits use warmer mystical crest effects.
- Compendium and run/top-panel icon art uses only Elesis' chibi head. Map movement art uses a compact red-and-gold Elesis pin with a small flame accent.

Do not treat generated art attempts as final until one is explicitly selected as the project asset.
