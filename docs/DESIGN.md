# Design

Confirmed design direction:

- Elesis should read as her base Elsword version, not a Grand Chase variant.
- Avoid advanced Elsword class-path details such as Grand Master, Blazing Heart, Bloody Queen, Empire Sword, or Flame Lord.
- Visual style should be simplified and adapted to Slay the Spire 2: dark fantasy, painterly, readable silhouettes, rough brush texture, and controlled crimson accents.
- Gameplay identity should start from sword-focused offense with restrained fire motifs, not from full evolution-class spectacle.
- The first implemented card pool uses prototype-safe card effects grouped by Vitality, Destruction, Flame, and Parry. Chivalry and Flame are implemented as counter powers; Parry is still a design label for efficient defensive cards.
- Card portraits have a first functional art pass. The 40 implemented cards each have runtime and big portrait PNGs named after their card IDs, with source art grouped by current mechanics and rarity beats.
- Character selection uses a Belder/Velder fortress backdrop with Elesis shown from head to mid-thigh, while the clickable selector icon stays focused on her head and shoulders and follows the template's `132x195` selector image size.
- The starter relic visual is the Belder Knight Emblem: a red-and-gold knight crest with a claymore motif on a transparent background, matching her red knight identity.
- Energy UI uses a red crystal-and-gold knight orb. Chivalry uses a red-gold sword crest power icon, and Flame uses a sharper red-orange sword-flame crest icon.
- Map marker art in solo and multiplayer reuses Elesis' compendium icon so her map presence matches the character icon.
- Combat, shop, and rest-site character art use transparent sprite assets with bottom-aligned scene placement. Combat art leans more chibi/readable than the larger menu illustrations so her feet align cleanly with other player visuals.
- Specialization art lives under `Elesis/images/specializations/` as `900x1000` transparent sprites. The four first-stage specialization sprites also have matching combat visual scenes so the selected form can replace Elesis' combat appearance for the rest of a run.
- Card portraits use the pre-v0.4.16 first functional art pass rather than the chibi redraw. Keep future card portrait experiments separate until explicitly selected.
- Compendium and run/top-panel icon art uses the restored pre-v0.4.17 Elesis icon.

Do not treat generated art attempts as final until one is explicitly selected as the project asset.
