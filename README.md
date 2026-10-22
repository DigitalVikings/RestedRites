# RestedRites

Channel the spirits of fallen beasts through the warmth of your hearth. **RestedRites** transforms mounted trophies into spiritual conduits, weaving their essence directly into your **Rested** status effect when you rest at home.

![Attuning a Mounted Trophy](https://raw.githubusercontent.com/DigitalVikings/RestedRites/main/screenshot_attune_ui.png)
![Active Rested Status HUD](https://raw.githubusercontent.com/DigitalVikings/RestedRites/main/screenshot_rested_ui.png)

> ⚠️ **Work in Progress (Pre-Release v0.1.0)**: This mod is in active development and has not been tested extensively. Features, balance, and mechanics may change, and unintended bugs or edge cases may occur. Backup your character and world saves before testing.

> ⚡ **Powered by VikingLib**: This mod requires [VikingLib](https://valheim.thunderstore.io/package/DigitalVikings/VikingLib/). 
> VikingLib replaces Valheim's native combat calculation and stat engines. Not all mods may be compatible / work as expected with this.

---

## How It Works

Mount creature trophies on item stands inside your home to gain passive rites and set synergies while **Rested**:

- **Attunement**: Mount any trophy on an Item Stand and press **`[Shift + E]`** to attune. Up to 4 trophies can be active simultaneously (FIFO slotting).
- **Comfort Progression**: Your home's Comfort level governs both active slot capacity and rite power.
  - **Slots**: Unlocks up to 4 active slots as Comfort reaches checkpoints (default: Comfort 1, 5, 10, 15).
  - **Power Budget**: Scales by +7.5% per Comfort level above 1, divided evenly across your active attuned slots.
- **Rested Duration**: Base duration of 10 minutes at Comfort 1, scaling by +20 seconds per Comfort level (or +60 seconds with the Abomination rite).
- **HUD Integration**: Active rites, creature synergies, and comfort bonuses display directly in the Rested status tooltip.

<details>
<summary><b>Click to expand Creature Sets (16 synergistic synergies)</b></summary>

| Set Name | Members | Milestone Bonuses |
| :--- | :--- | :--- |
| **Pioneers of the Wilds** | Boar, Deer, Neck, Hare | **(2)** +10% Sprint Stamina Efficiency under 50% carry weight<br>**(3)** +5% Foraging double yield & drop rolls<br>**(4)** +25% Movement speed |
| **The Restless Barrow** | Skeleton, Rancid Remains, Ghost, Wraith | **(2)** -35% Piercing damage taken<br>**(3)** +1.0x Sneak attack multiplier<br>**(4)** +20% Sneak movement speed & +35% Sneak Stamina Efficiency |
| **The Earthshakers** | Troll, Stone Golem, Morgen, Jötunn Warrior | **(2)** +30% Pickaxe damage to stone & ore<br>**(3)** +25% Knockback dealt & stagger power<br>**(4)** +35% Cleave damage efficiency |
| **The Plains Tribe** | Fuling, Surtling, Wolf, Growth | **(2)** +25% Damage vs afflicted targets (tarred, slowed, staggered)<br>**(3)** +15 Max Stamina & +20% Dodge Stamina Efficiency<br>**(4)** Adds +15% Fire damage to all physical attacks |
| **Beastmasters of the Hearth** | Lox, Asksvin, Bear, Moose | **(2)** +75% Stamina regen while standing still<br>**(3)** +75% Cart Cargo Efficiency & +25% Tool Stamina Efficiency<br>**(4)** +50% Mount Stamina Efficiency & +20% Mount Speed |
| **Weavers of Eitr** | Cultist, Gjall, Fallen Valkyrie, Jötunn Witch | **(2)** +15% Eitr regen & +10% Spell Eitr Efficiency<br>**(3)** +20 Max Eitr<br>**(4)** +25% Spell Eitr Efficiency |
| **Children of the Forest** | Greydwarf, Greydwarf Brute, Greydwarf Shaman | **(2)** +20% Wood yield from trees<br>**(3)** +20% Health regen within 20m of a tree |
| **The Draugr Revenants** | Draugr, Draugr Elite, Vile | **(2)** +40% Stagger buildup on sneak attacks<br>**(3)** Last Stand cooldown halved (5 min) & restores full stamina |
| **The Gelatinous Horde** | Blob, Frost Blob, Lava Blob | **(2)** +25% Jump height & -50% Fall damage taken<br>**(3)** Slime Splash: Melee attackers are slowed for 4s |
| **The Bloodbound** | Bat, Leech, Charred Warrior | **(2)** +20 Stamina restored on kill<br>**(3)** 2x Damage on your next attack after killing an enemy |
| **The Hive Chitin** | Seeker, Seeker Soldier, Tick | **(2)** +10 Base Armor<br>**(3)** Carapace Counter: Reflects 50% damage when Carapace Ward triggers |
| **Underground Delvers** | Dvergr, Mole, Barka | **(2)** +75% Tool & Weapon Durability Efficiency<br>**(3)** +20% Mining yield & +10 Armor in crypts and dungeons |
| **Tidecallers of the Abyss** | Serpent, Bonemaw Serpent, Seal | **(2)** +50% Swim Stamina Efficiency & -30% Damage taken in water<br>**(3)** +15% Ship sailing speed |
| **Wings of Prey** | Drake, Deathsquito, Volture | **(2)** +15% Piercing damage & +20% Shield Block Stamina Efficiency<br>**(3)** +35% Attack damage from mid-air or elevation |
| **The Goblin Chieftains** | Fuling Berserker, Fuling Shaman | **(2)** +15 Stamina restored upon staggering an enemy |
| **The Fenris Kin** | Fenring, Ulv | **(2)** +30% Damage on 3rd hit of weapon attack combos |
| **The Charred Duo** | Charred Marksman, Charred Warlock | **(2)** +25% Damage dealt with elemental arrows |

</details>

<details>
<summary><b>Click to expand Standalone Wildcards (non-set exotic rites)</b></summary>

- **Abomination (Ancient Slumber)**: Increases Rested duration scaling from +20s to **+60s (+1 minute)** per Comfort level.
- **Brenna (Brenna's Pyre)**: Critical strikes and sneak attacks detonate a 4m **Fireball Blast** dealing 25% of hit damage as AoE Fire.
- **Geirrhafa (Mountain Prowler)**: +25% Dodge Stamina Efficiency.
- **Zil (Shamanic Aegis)**: -9% Physical and Elemental damage taken (compounding damage reduction).
- **Steil (Unstoppable Poise)**: +60% Stagger resistance threshold.

</details>

<details>
<summary><b>Click to expand full 55 Trophy Roster</b></summary>

| Biome | Creature | Rite Name | Base Power | Effect |
| :--- | :--- | :--- | :--- | :--- |
| **Meadows** | Boar | Boar's Burden | +40 kg | Carry weight |
| **Meadows** | Deer | Stag's Stride | 30% | Run stamina efficiency |
| **Meadows** | Neck | Forager's Eye | 15% | Foraging double gather chance |
| **Black Forest** | Greydwarf | Timberjack | 28% | Wood chopping damage |
| **Black Forest** | Greydwarf Brute | Brute Force | 18% | Blunt damage dealt |
| **Black Forest** | Greydwarf Shaman | Regrowth | 30% | Health regeneration rate |
| **Black Forest** | Skeleton | Deadeye | 22% | Ranged bow/crossbow damage |
| **Black Forest** | Rancid Remains | Festering Touch | 28% | Physical damage applied as Poison DoT over 5s |
| **Black Forest** | Troll | Giant's Reach | 15% | Melee attack reach |
| **Black Forest** | Ghost | Ethereal Form | 20% | Physical damage taken reduction |
| **Swamp** | Abomination | Ancient Slumber | +60s/lvl | Increases duration scaling to +1m per Comfort level |
| **Swamp** | Blob | Gelatinous | +45 HP | Maximum health pool |
| **Swamp** | Draugr | Ambush | +1.5x | Sneak attack damage multiplier |
| **Swamp** | Draugr Elite | Warrior's Edge | 18% | Slashing damage dealt |
| **Swamp** | Leech | Sanguine Harvest | 2 HP | Health restored on enemy kill |
| **Swamp** | Surtling | Fireward | 25% | Fire damage taken reduction |
| **Swamp** | Wraith | Gliding Shroud | 80% / 10% | -80% Fall damage & +10% Sneak movement speed |
| **Mountains** | Bat | Nocturnal Hunter | 28% | Damage bonus at night |
| **Mountains** | Cultist | Inner Flame | 65% | Eitr regeneration rate |
| **Mountains** | Drake | Frostveil | 25% | Frost damage taken reduction |
| **Mountains** | Fenring | Predator's Rush | 25% | Movement speed boost for 3s on hit |
| **Mountains** | Stone Golem | Crag Shatter | 28% | Pickaxe damage to stone & ore |
| **Mountains** | Ulv | Feral Haste | 10% | Melee attack swing speed |
| **Mountains** | Wolf | Pack Pace | 18% | Movement speed |
| **Plains** | Deathsquito | Needle Sting | 18% | Piercing damage dealt |
| **Plains** | Fuling | Opportunist | 35% | Damage vs staggered targets |
| **Plains** | Fuling Berserker | Unbridled Fury | 20% | Stagger buildup dealt |
| **Plains** | Fuling Shaman | Totemic Endurance | +45 | Maximum stamina pool |
| **Plains** | Growth | Tar Coating | 60% / 30% | Heavy / Light attack proc chance to Tar foes (8s) |
| **Plains** | Lox | Laborer's Mettle | 75% | Tool stamina efficiency |
| **Plains** | Vile | Last Stand | 25% | Defy fatal hit with 25% max HP restored (10m CD) |
| **Ocean** | Serpent | Leviathan Glide | 50% | Swim speed |
| **Mistlands** | Hare | Bunny's Hop | 50% | Jump stamina efficiency |
| **Mistlands** | Gjall | Gjall's Inferno | 18% | Elemental magic damage |
| **Mistlands** | Seeker | Chitin Breaker | 22% | Physical damage vs resistant targets |
| **Mistlands** | Seeker Soldier | Carapace Ward | 80% / $90\text{s}/\text{pwr}$ | Absorbs 80% damage on first incoming hit |
| **Mistlands** | Tick | Spare Vitality | 40% | Blood magic health efficiency |
| **Mistlands** | Dvergr | Dvergr's Lantern | 30% | Mining extraction yield |
| **Ashlands** | Asksvin | Cavalry Charge | 30% | Mount speed & stamina efficiency |
| **Ashlands** | Bonemaw Serpent | Sea Drake's Wake | 35% | Ship sailing speed |
| **Ashlands** | Fallen Valkyrie | Valkyrie's Aether | +45 Eitr | Maximum Eitr pool |
| **Ashlands** | Charred Marksman | Snapdraw | 35% | Bow draw & reload speed |
| **Ashlands** | Morgen | Morgen's Cull | 40% | Damage vs enemies below 35% HP |
| **Ashlands** | Volture | Wingshield | 40% | Block stamina efficiency |
| **Ashlands** | Charred Warlock | Cinder Shield | 15% | Elemental damage taken reduction |
| **Ashlands** | Charred Warrior | Juggernaut | 15% | Two-handed weapon damage |
| **Deep North** | Bear | Ursine Stamina | 60% | Stamina regeneration rate |
| **Deep North** | Seal | Aquatic Endurance | 50% | Swim stamina efficiency |
| **Deep North** | Moose | Draft Hauler | 55% | Cart cargo efficiency |
| **Deep North** | Mole | Subterranean Greed | 22% | Mining yield & duplicate drop rolls |
| **Deep North** | Barka | Frosted Hide | +15 Armor | Base armor |
| **Deep North** | Frost Blob | Numbing Touch | 60% / 30% | Heavy / Light attack proc chance to Chill foes (8s) |
| **Deep North** | Lava Blob | Warm Hearth | 50% | Food buff duration |
| **Deep North** | Jötunn Warrior | Titan's Swat | 40% | Knockback force dealt |
| **Deep North** | Jötunn Witch | Rime Weaver | 25% | Eitr spell efficiency |
| **Hildir** | Brenna | Brenna's Pyre | 25% | Crits & sneak hits trigger an AoE Fireball blast |
| **Hildir** | Geirrhafa | Mountain Prowler | 25% | Dodge stamina efficiency |
| **Hildir** | Zil | Shamanic Aegis | 9% | Physical & elemental damage reduction |
| **Hildir** | Steil | Unstoppable Poise | 60% | Stagger poise threshold |

</details>
