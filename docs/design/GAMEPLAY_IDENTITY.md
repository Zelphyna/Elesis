# Gameplay Identity

Use this file for the character's confirmed mechanical identity, archetypes, and constraints.

- Elesis uses Chivalry as her primary combat rhythm. Vitality cards build Chivalry, Destruction cards can spend 5 Chivalry for bonus damage or Block, and Vitality thresholds can spend 5 Chivalry to draw.
- Flame is a secondary burst counter with a dedicated visible power icon. Flame cards build Flame; non-Flame Attacks consume stored Flame as bonus damage.
- Power cards should grant Chivalry or Flame as start-of-turn flow instead of one-time immediate resource bursts.
- Card descriptions color Flame with an orange highlight and Destruction with a red highlight to separate the mechanics visually.
- Belder Knight Emblem is Elesis' starter relic. It gives 2 Chivalry at the start of each combat, giving her an early rhythm without immediately triggering the 5-Chivalry threshold.
- Starting deck uses `QuickStep` and `ClaymoreArc` as starter-only Basic cards so a new run can build Chivalry and immediately see a Destruction spend pattern without those cards appearing in normal rewards or shop rolls.
- The card pool must include at least one shop-eligible Power card. The vanilla merchant card layout includes a Power slot for the current character, so Elesis keeps Power cards available across rarities to avoid empty shop generation.
- Belder Knight Emblem tracks run-local specialization experience. It starts at 0 each run. Combat XP is offered as a selectable post-combat reward: normal combats grant 3 XP, elites grant 4 XP, and bosses grant 6 XP. Non-combat nodes do not grant specialization XP. At 15 XP, once the map is reopened, Elesis chooses Saber Knight, Pyro Knight, Dark Knight, or Soar Knight for the rest of the run.
- Specialization effects scale with the chosen branch's evolution tier: tier 1 at 15 XP grants +1, tier 2 at 35 XP grants +2, and tier 3 at 55 XP grants +3. Saber Knight improves Chivalry gains, Pyro Knight improves Flame gains, Dark Knight adds attack damage, and Soar Knight adds Block gained.
