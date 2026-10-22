# Changelog

All notable changes to **RestedRites** will be documented in this file.

## [2.0.0] - 2026-10-18

### Major Architecture & Engine Overhaul (VikingLib Integration)
* **VikingLib Combat & Stats Architecture**:
  * Fully integrated with VikingLib's 6-phase `DamagePipeline` for combat damage, tool interactions (including Greydwarf Timberjack chopping bonuses), and sneak/ambush multipliers.
  * Standardized all stat and resistance bonuses on VikingLib's 3-tier mathematical engine (Tier 1 Flat, Tier 2 Additive %, Tier 3 Compounding Multipliers).
  * Isolated native Valheim skill calculations from leaking into custom rite modifiers.
  * Standardized stamina mechanics to positive efficiency semantics (+% Run Stamina Efficiency, +% Tool Stamina Efficiency) matching Hugin's compendium and player expectations.
* **Duration & Abomination Rite Rebalance**:
  * Rested duration now begins at **10 minutes** at Comfort 1, increasing by **+20 seconds** per Comfort level above 1.
  * Attuning the **Abomination** trophy (Ancient Slumber) elevates this scaling to **+1 minute per Comfort level** (+60s), rewarding deep marsh hunts with prolonged expeditions.
  * Fixed baseline Rested buff toggling in server and local configuration to cleanly isolate custom rite bonuses from vanilla baseline multipliers.
* **Refined Sensory Polish**:
  * Shortened attunement particle burst to a snappy, crisp 0.2s offering dissipation burst.
  * Removed lingering purple mist from the passive Rested status effect, preserving pristine player visibility while resting.
* **Jötunn Server Synchronization**:
  * Enforced server-authoritative configuration via Jötunn's `SynchronizationManager`.
  * Added `[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]` for guaranteed multi-client stability.
  * Subscribed to `OnConfigurationSynchronized` to hot-reapply active Rested rites and stat bonuses immediately whenever server config updates.

## [0.1.0] - 2026-10-08

### Initial Release Preview
* **59 Trophy Rites**: Full mechanical support across all vanilla biomes (Meadows, Black Forest, Swamp, Mountains, Plains, Ocean, Mistlands, Ashlands, Deep North) plus Hildir's Request mini-bosses.
* **Comfort-Scaled Attunement**:
  * Unlocks active attunement slots dynamically based on player Comfort level checkpoints (defaults: Comfort 1, 5, 10, 15).
  * Global power budget scales dynamically ($+7.5\%$ per comfort level above 1).
  * Equal power division across all slotted rites ($\text{Power} = \frac{\text{Total Power Pool}}{\text{Attuned Slots}} \times \text{Base Magnitude}$).
* **16 Thematic Creature Sets**:
  * Combining trophies from related creatures unlocks powerful 2-piece, 3-piece, and 4-piece set synergies (e.g. *The Bloodbound*, *The Hive Chitin*, *Pioneers of the Wilds*, *The Earthshakers*, *The Draugr Revenants*).
* **5 Standalone Wildcards**:
  * Unique standalone mechanics for trophies without sets: **Abomination** (Ancient Slumber), **Brenna** (Brenna's Pyre), **Geirrhafa** (Mountain Prowler), **Zil** (Shamanic Aegis), and **Steil** (Unstoppable Poise).
* **Sensory Polish ("Sensory Juice")**:
  * Golden offering burst and altar chime upon attuning trophies with `[Shift + E]`.
  * Visual block deflection ward and metallic clank on Seeker Soldier Carapace Ward.
  * Rejuvenation surge on Vile Last Stand death defiance.
  * Combustion fireball explosion on Brenna critical hits.
  * Frost fog mist on Geirrhafa dodge rolls.
  * Shamanic golden ward chime when Zil absorbs harmful debuffs.
  * Mead stamina visual effect on kill/stagger stamina procs.
* **Hugin Compendium & Dynamic UI**:
  * Displays active attunements and exact mathematical scaling factors in the Compendium and Active Effects tab.
  * Dynamic weapon and tool tooltips reflecting attuned damage and stat buffs.
  * Standalone Rested tooltip clean-up preventing rite bleed-through.
* **Dedicated Server Synchronization**:
  * Admin-only enforcement and automated client synchronization powered by Jötunn.
