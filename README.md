# RestedRites

Channel the spirits of fallen beasts through the warmth of your hearth. **RestedRites** transforms mounted trophies into spiritual conduits, weaving their essence directly into your **Rested** status effect when you rest at home.

![Attuning a Mounted Trophy](https://raw.githubusercontent.com/DigitalVikings/RestedRites/main/screenshot_attune_ui.png)
![Active Rested Status HUD](https://raw.githubusercontent.com/DigitalVikings/RestedRites/main/screenshot_rested_ui.png)

> ⚡ **Powered by VikingLib**: This mod requires [VikingLib](https://valheim.thunderstore.io/package/DigitalVikings/VikingLib/). 
> VikingLib replaces Valheim's native combat calculation and stat engines. Not all mods may be compatible / work as expected with this.

---

## How It Works

Mount creature trophies on item stands inside your home to gain passive rites and set synergies while **Rested**:

- **Early-Game Stands**: Item stands (both vertical and horizontal) require only **4 Wood + 1 Leather Scraps** at a Workbench, enabling trophy mounting on Day 1 in the Meadows.
- **Attunement**: Mount any trophy on an Item Stand and press **[Shift + E]** to attune. Up to 4 trophies can be active simultaneously. The most recently attuned trophy becomes your **Primary Anchor** (Slot 1), shifting previous trophies into supporting Aspect slots (Slots 2–4).
- **Comfort Progression (24x4 Progression Matrix)**: Your home's Comfort level governs slot capacity and per-slot power scaling across Comfort levels 1 to 24.
  - **Reservoir Spill-Over Model (Default)**: Your Primary Anchor starts at **100% power** and never degrades. Surplus comfort power cascades sequentially into supporting slots (unlocking at Comfort 2, 8, and 15), activating early set bonuses without ever diluting your main trophy.
  - **Uniform Power Model**: Power budget grows by +7.5% per comfort level and is shared evenly across all active slots.
  - **Custom Model**: Fully configurable via a 24x4 matrix in the configuration file.
- **Rested Duration**: Base duration of 10 minutes at Comfort 1, scaling by +20 seconds per Comfort level (or +60 seconds with the Abomination rite).
- **HUD & Compendium Integration**: Active rites display with their specific slot tier ([Anchor: 100%], [Slot 2: 30%]), comfort scaling, and creature synergies directly in the Rested status tooltip and Hugin's Compendium.

<details>
<summary><b>Click to expand Creature Sets (16 synergistic synergies)</b></summary>

| Set Name | Members | Milestone Bonuses |
| :--- | :--- | :--- |
| **Pioneers of the Wilds** | Boar, Deer, Neck, Hare | **(2)** +15% Sprint Stamina Efficiency under 50% carry weight<br>**(3)** +5% Foraging double yield & drop rolls<br>**(4)** +25% Movement speed |
| **The Restless Barrow** | Skeleton, Rancid Remains, Ghost, Wraith | **(2)** -35% Piercing damage taken<br>**(3)** +1.0x Sneak attack multiplier<br>**(4)** +20% Sneak movement speed & +35% Sneak Stamina Efficiency |
| **The Earthshakers** | Troll, Stone Golem, Morgen, Jötunn Warrior | **(2)** +40% Pickaxe damage to stone & ore<br>**(3)** +25% Knockback dealt & stagger power<br>**(4)** +35% Cleave damage efficiency |
| **The Plains Tribe** | Fuling, Surtling, Wolf, Growth | **(2)** +25% Damage vs afflicted targets (tarred, slowed, staggered)<br>**(3)** +25 Max Stamina & +20% Dodge Stamina Efficiency<br>**(4)** Adds +15% Fire damage to all physical attacks |
| **Beastmasters of the Hearth** | Lox, Asksvin, Bear, Moose | **(2)** +75% Stamina regen while standing still<br>**(3)** +75% Cart Cargo Efficiency & +25% Tool Stamina Efficiency<br>**(4)** +50% Mount Stamina Efficiency & +20% Mount Speed |
| **Weavers of Eitr** | Cultist, Gjall, Fallen Valkyrie, Jötunn Witch | **(2)** +15% Eitr regen & +10% Spell Eitr Efficiency<br>**(3)** +30 Max Eitr<br>**(4)** +25% Spell Eitr Efficiency |
| **Children of the Forest** | Greydwarf, Greydwarf Brute, Greydwarf Shaman | **(2)** +20% Wood yield from trees<br>**(3)** +20% Health regen within 20m of a tree |
| **The Draugr Revenants** | Draugr, Draugr Elite, Vile | **(2)** +40% Stagger buildup on sneak attacks<br>**(3)** Last Stand cooldown halved (5 min) & restores full stamina |
| **The Gelatinous Horde** | Blob, Frost Blob, Lava Blob | **(2)** +25% Jump height & -50% Fall damage taken<br>**(3)** Slime Splash: Melee attackers are slowed for 4s |
| **The Bloodbound** | Bat, Leech, Charred Warrior | **(2)** +20 Stamina restored on kill<br>**(3)** 2x Damage on your next attack after killing an enemy |
| **The Hive Chitin** | Seeker, Seeker Soldier, Tick | **(2)** +15 Base Armor<br>**(3)** Carapace Counter: Reflects 50% damage when Carapace Ward triggers |
| **Underground Delvers** | Dvergr, Mole, Barka | **(2)** +100% Tool & Weapon Durability Efficiency<br>**(3)** +20% Mining yield & +10 Armor in crypts and dungeons |
| **Tidecallers of the Abyss** | Serpent, Bonemaw Serpent, Seal | **(2)** +50% Swim Stamina Efficiency & -30% Damage taken in water<br>**(3)** +15% Ship sailing speed |
| **Wings of Prey** | Drake, Deathsquito, Volture | **(2)** +15% Piercing damage & +20% Shield Block Stamina Efficiency<br>**(3)** +35% Attack damage from mid-air or elevation |
| **The Goblin Chieftains** | Fuling Berserker, Fuling Shaman | **(2)** +15 Stamina restored upon staggering an enemy |
| **The Fenris Kin** | Fenring, Ulv | **(2)** +30% Damage on 3rd hit of weapon attack combos |
| **The Charred Duo** | Charred Marksman, Charred Warlock | **(2)** +25% Damage dealt with elemental arrows |

</details>

<details>
<summary><b>Click to expand Standalone Wildcards (non-set exotic rites)</b></summary>

- **Abomination (Ancient Slumber)**: Increases Rested duration scaling from +20s to **+60s (+1 minute)** per Comfort level.
- **Brenna (Brenna's Pyre)**: Critical strikes and sneak attacks detonate a 4m **Fireball Blast** dealing 35% of hit damage as AoE Fire.
- **Geirrhafa (Mountain Prowler)**: +35% Dodge Stamina Efficiency.
- **Zil (Shamanic Aegis)**: -12% Physical and Elemental damage taken (compounding damage reduction).
- **Steil (Unstoppable Poise)**: +75% Stagger resistance threshold.

</details>

<details>
<summary><b>Click to expand full 55 Trophy Roster</b></summary>

| Biome | Creature | Rite Name | Base Power | Effect |
| :--- | :--- | :--- | :--- | :--- |
| **Meadows** | Boar | Gluttonous Digestion | 50% | Food buff duration |
| **Meadows** | Deer | Stag's Stride | 60% | Stamina regeneration rate |
| **Meadows** | Neck | Forager's Eye | 25% | Foraging double gather chance |
| **Black Forest** | Greydwarf | Timberjack | 40% | Wood chopping damage |
| **Black Forest** | Greydwarf Brute | Brute Force | 30% | Blunt damage dealt |
| **Black Forest** | Greydwarf Shaman | Regrowth | 50% | Health regeneration rate |
| **Black Forest** | Skeleton | Deadeye | 30% | Ranged bow/crossbow damage |
| **Black Forest** | Rancid Remains | Festering Touch | 40% | Physical damage applied as Poison DoT over 5s |
| **Black Forest** | Troll | Giant's Reach | 20% | Melee attack reach |
| **Black Forest** | Ghost | Ethereal Form | 25% | Physical damage taken reduction |
| **Swamp** | Abomination | Ancient Slumber | +60s/lvl | Increases duration scaling to +1m per Comfort level |
| **Swamp** | Blob | Gelatinous | +60 HP | Maximum health pool |
| **Swamp** | Draugr | Ambush | +2.0x | Sneak attack damage multiplier |
| **Swamp** | Draugr Elite | Warrior's Edge | 30% | Slashing damage dealt |
| **Swamp** | Leech | Sanguine Harvest | 6 HP | Health restored on enemy kill |
| **Swamp** | Surtling | Fireward | 30% | Fire damage taken reduction |
| **Swamp** | Wraith | Gliding Shroud | -90% / +50% | -90% Fall damage & +50% Jump height |
| **Mountains** | Bat | Nocturnal Hunter | 35% | Damage bonus at night |
| **Mountains** | Cultist | Inner Flame | 70% | Eitr regeneration rate |
| **Mountains** | Drake | Frostveil | 30% | Frost damage taken reduction |
| **Mountains** | Fenring | Predator's Rush | 35% | Movement speed boost for 4s on hit |
| **Mountains** | Stone Golem | Crag Shatter | 40% | Pickaxe damage to stone & ore |
| **Mountains** | Ulv | Feral Haste | 15% | Melee attack swing speed |
| **Mountains** | Wolf | Pack Pace | 20% | Movement speed |
| **Plains** | Deathsquito | Needle Sting | 30% | Piercing damage dealt |
| **Plains** | Fuling | Opportunist | 50% | Damage vs staggered targets |
| **Plains** | Fuling Berserker | Unbridled Fury | 40% | Stagger buildup dealt |
| **Plains** | Fuling Shaman | Totemic Endurance | +60 | Maximum stamina pool |
| **Plains** | Growth | Tar Coating | 75% / 40% | Heavy / Light attack proc chance to Tar foes (8s) |
| **Plains** | Lox | Laborer's Mettle | 100% | Tool stamina efficiency |
| **Plains** | Vile | Last Stand | 35% | Defy fatal hit with 35% max HP restored (10m CD) |
| **Ocean** | Serpent | Leviathan Glide | 100% | Swim speed |
| **Mistlands** | Hare | Bunny's Hop | 100% | Jump stamina efficiency |
| **Mistlands** | Gjall | Gjall's Inferno | 30% | Elemental magic damage |
| **Mistlands** | Seeker | Razor Precision | 25% | Critical strike chance |
| **Mistlands** | Seeker Soldier | Carapace Ward | 80% / 75s/pwr | Absorbs 80% damage on first incoming hit |
| **Mistlands** | Tick | Spare Vitality | 50% | Blood magic health efficiency |
| **Mistlands** | Dvergr | Dvergr's Wisdom | 50% | Skill experience gain |
| **Ashlands** | Asksvin | Cavalry Charge | 40% | Mount speed & stamina efficiency |
| **Ashlands** | Bonemaw Serpent | Sea Drake's Wake | 45% | Ship sailing speed |
| **Ashlands** | Fallen Valkyrie | Valkyrie's Aether | +60 Eitr | Maximum Eitr pool |
| **Ashlands** | Charred Marksman | Snapdraw | 45% | Bow draw & reload speed |
| **Ashlands** | Morgen | Morgen's Cull | 50% | Damage vs enemies below 35% HP |
| **Ashlands** | Volture | Wingshield | 75% | Block stamina efficiency |
| **Ashlands** | Charred Warlock | Cinder Shield | 20% | Elemental damage taken reduction |
| **Ashlands** | Charred Warrior | Juggernaut | 25% | Two-handed weapon damage |
| **Deep North** | Bear | Ursine Ferocity | 50% | Berserk damage when below 35% HP |
| **Deep North** | Seal | Aquatic Endurance | 100% | Swim stamina efficiency |
| **Deep North** | Moose | Draft Hauler | 75% | Cart cargo efficiency |
| **Deep North** | Mole | Subterranean Greed | 30% | Mining yield & duplicate drop rolls |
| **Deep North** | Barka | Frosted Hide | +25 Armor | Base armor |
| **Deep North** | Frost Blob | Numbing Touch | 75% / 40% | Heavy / Light attack proc chance to Chill foes (8s) |
| **Deep North** | Lava Blob | Furnace Core | 25% | Cooking extra yield |
| **Deep North** | Jotunn Warrior | Titan's Swat | 60% | Knockback force dealt |
| **Deep North** | Jotunn Witch | Rime Weaver | 35% | Eitr spell efficiency |
| **Hildir** | Brenna | Brenna's Pyre | 35% | Crits & sneak hits trigger an AoE Fireball blast |
| **Hildir** | Geirrhafa | Mountain Prowler | 35% | Dodge stamina efficiency |
| **Hildir** | Zil | Shamanic Aegis | 12% | Physical & elemental damage reduction |
| **Hildir** | Steil | Unstoppable Poise | 75% | Stagger poise threshold |

</details>