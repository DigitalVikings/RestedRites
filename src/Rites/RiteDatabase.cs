using System;
using System.Collections.Generic;
using VikingLib.Stats;

namespace RestedRites.Rites
{
    public static class RiteDatabase
    {
        private static readonly Dictionary<string, TrophyRite> ByPrefab = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<TrophyId, TrophyRite> ById = new();

        private static RiteModifierFactory Flat(PhysicalStat stat, string label) =>
            (p, val, owner) => new[] { StatModifier.Flat(stat, val, label, null, owner) };

        private static RiteModifierFactory Increased(StatToken stat, string label, float factor = 0.01f) =>
            (p, val, owner) => new[] { StatModifier.Increased(stat, val * factor, label, null, owner) };

        private static RiteModifierFactory Multiplier(BaseStatToken stat, string label, float factor = 0.01f) =>
            (p, val, owner) => new[] { StatModifier.Multiplier(stat, 1f + val * factor, label, null, owner) };

        private static RiteModifierFactory Resistance(ResistanceStat stat, string label, float factor = 0.01f) =>
            (p, val, owner) => new[] { StatModifier.Multiplier(stat, Math.Max(0f, 1f - val * factor), label, null, owner) };

        static RiteDatabase()
        {
            // Meadows
            Register(new TrophyRite(TrophyId.Boar, "TrophyBoar", "Gluttonous Digestion", "Boar", "Meadows", 50f, "%", "+{0}{1} Food Duration",
                Increased(CharacterStats.FoodDurationMultiplier, "Gluttonous Digestion")));
            Register(new TrophyRite(TrophyId.Deer, "TrophyDeer", "Stag's Stride", "Deer", "Meadows", 60f, "%", "+{0}{1} Stamina Regeneration Rate",
                Increased(CharacterStats.StaminaRegen, "Stag's Stride")));
            Register(new TrophyRite(TrophyId.Neck, "TrophyNeck", "Forager's Eye", "Neck", "Meadows", 25f, "%", "{0}{1} Double Gather Chance",
                Increased(CharacterStats.ForagingYield, "Forager's Eye")));

            // Black Forest
            Register(new TrophyRite(TrophyId.Greydwarf, "TrophyGreydwarf", "Timberjack", "Greydwarf", "Black Forest", 40f, "%", "+{0}{1} Wood Chopping Damage",
                Increased(CharacterStats.DamageChop, "Timberjack")));
            Register(new TrophyRite(TrophyId.GreydwarfBrute, "TrophyGreydwarfBrute", "Brute Force", "Greydwarf Brute", "Black Forest", 30f, "%", "+{0}{1} Blunt Damage Dealt",
                Increased(CharacterStats.DamageBlunt, "Brute Force")));
            Register(new TrophyRite(TrophyId.GreydwarfShaman, "TrophyGreydwarfShaman", "Regrowth", "Greydwarf Shaman", "Black Forest", 50f, "%", "+{0}{1} Health Regeneration",
                Increased(CharacterStats.HealthRegen, "Regrowth")));
            Register(new TrophyRite(TrophyId.Skeleton, "TrophySkeleton", "Deadeye", "Skeleton", "Black Forest", 30f, "%", "+{0}{1} Ranged Damage Dealt",
                Increased(CharacterStats.DamageRanged, "Deadeye"), "TrophySkeletonHildir"));
            Register(new TrophyRite(TrophyId.RancidRemains, "TrophySkeletonPoison", "Festering Touch", "Rancid Remains", "Black Forest", 40f, "%", "+{0}{1} Physical Hit as Poison DoT (5s)",
                Increased(CharacterStats.DamagePoison, "Festering Touch")));
            Register(new TrophyRite(TrophyId.Troll, "TrophyFrostTroll", "Giant's Reach", "Troll", "Black Forest", 20f, "%", "+{0}{1} Melee Reach",
                Multiplier(CharacterStats.MeleeReach, "Giant's Reach"), "TrophyForestTroll"));
            Register(new TrophyRite(TrophyId.Ghost, "TrophyGhost", "Ethereal Form", "Ghost", "Black Forest", 25f, "%", "-{0}{1} Physical Damage Taken",
                Resistance(CharacterStats.ResistancePhysical, "Ethereal Form")));

            // Swamp
            Register(new TrophyRite(TrophyId.Abomination, "TrophyAbomination", "Ancient Slumber", "Abomination", "Swamp", 1f, "m", "+{0}{1} Rested Duration per Comfort Level",
                null)); // Handled dynamically in SERestedPatch duration
            Register(new TrophyRite(TrophyId.Blob, "TrophyBlob", "Gelatinous", "Blob", "Swamp", 60f, "HP", "+{0} Max Health",
                Flat(CharacterStats.MaxHealth, "Gelatinous")));
            Register(new TrophyRite(TrophyId.Draugr, "TrophyDraugr", "Ambush", "Draugr", "Swamp", 2.0f, "x", "+{0}x Sneak Attack Multiplier",
                Increased(CharacterStats.DamageSneak, "Ambush", 1.0f), "TrophyDraugrFem"));
            Register(new TrophyRite(TrophyId.DraugrElite, "TrophyDraugrElite", "Warrior's Edge", "Draugr Elite", "Swamp", 30f, "%", "+{0}{1} Slashing Damage Dealt",
                Increased(CharacterStats.DamageSlashing, "Warrior's Edge")));
            Register(new TrophyRite(TrophyId.Leech, "TrophyLeech", "Sanguine Harvest", "Leech", "Swamp", 6f, "HP", "+{0} HP on Kill",
                Flat(CharacterStats.HealthOnKill, "Sanguine Harvest")));
            Register(new TrophyRite(TrophyId.Surtling, "TrophySurtling", "Fireward", "Surtling", "Swamp", 30f, "%", "-{0}{1} Fire Damage Taken",
                Resistance(CharacterStats.ResistanceFire, "Fireward")));
            Register(new TrophyRite(TrophyId.Wraith, "TrophyWraith", "Gliding Shroud", "Wraith", "Swamp", 90f, "%", "-{0}{1} Fall Damage & +50% Jump Height",
                (p, val, owner) => new[] {
                    StatModifier.Multiplier(CharacterStats.ResistanceFall, Math.Max(0f, 1f - val * 0.01f), "Gliding Shroud", null, owner),
                    StatModifier.Increased(CharacterStats.JumpHeight, 0.50f * (val / 90f), "Gliding Shroud", null, owner)
                }));

            // Mountains
            Register(new TrophyRite(TrophyId.Bat, "TrophyBat", "Nocturnal Hunter", "Bat", "Mountains", 35f, "%", "+{0}{1} Damage at Night",
                Increased(CharacterStats.DamageAtNight, "Nocturnal Hunter")));
            Register(new TrophyRite(TrophyId.Cultist, "TrophyCultist", "Inner Flame", "Cultist", "Mountains", 70f, "%", "+{0}{1} Eitr Regeneration",
                Increased(CharacterStats.EitrRegen, "Inner Flame"), "TrophyCultistHildir"));
            Register(new TrophyRite(TrophyId.Drake, "TrophyHatchling", "Frostveil", "Drake", "Mountains", 30f, "%", "-{0}{1} Frost Damage Taken",
                Resistance(CharacterStats.ResistanceFrost, "Frostveil")));
            Register(new TrophyRite(TrophyId.Fenring, "TrophyFenring", "Predator's Rush", "Fenring", "Mountains", 35f, "%", "+{0}{1} Move Speed for 4s on Hit",
                null)); // Handled via DamagePipeline combat hook
            Register(new TrophyRite(TrophyId.StoneGolem, "TrophySGolem", "Crag Shatter", "Stone Golem", "Mountains", 40f, "%", "+{0}{1} Pickaxe Damage to Stone & Ore",
                Increased(CharacterStats.DamagePickaxe, "Crag Shatter")));
            Register(new TrophyRite(TrophyId.Ulv, "TrophyUlv", "Feral Haste", "Ulv", "Mountains", 15f, "%", "+{0}{1} Melee Attack Swing Speed",
                Increased(CharacterStats.AttackSpeed, "Feral Haste")));
            Register(new TrophyRite(TrophyId.Wolf, "TrophyWolf", "Pack Pace", "Wolf", "Mountains", 20f, "%", "+{0}{1} Movement Speed",
                (p, val, owner) => new[] {
                    StatModifier.Increased(CharacterStats.JogSpeed, val * 0.01f, "Pack Pace", null, owner),
                    StatModifier.Increased(CharacterStats.RunSpeed, val * 0.01f, "Pack Pace", null, owner)
                }));

            // Plains
            Register(new TrophyRite(TrophyId.Deathsquito, "TrophyDeathSquito", "Needle Sting", "Deathsquito", "Plains", 30f, "%", "+{0}{1} Piercing Damage Dealt",
                Increased(CharacterStats.DamagePiercing, "Needle Sting")));
            Register(new TrophyRite(TrophyId.Fuling, "TrophyGoblin", "Opportunist", "Fuling", "Plains", 50f, "%", "+{0}{1} Damage vs Staggered Targets",
                Increased(CharacterStats.DamageAgainstStaggered, "Opportunist")));
            Register(new TrophyRite(TrophyId.FulingBerserker, "TrophyGoblinBrute", "Unbridled Fury", "Fuling Berserker", "Plains", 40f, "%", "+{0}{1} Stagger Buildup Dealt",
                Increased(CharacterStats.StaggerDamage, "Unbridled Fury"), "TrophyGoblinBruteBrosShaman"));
            Register(new TrophyRite(TrophyId.FulingShaman, "TrophyGoblinShaman", "Totemic Endurance", "Fuling Shaman", "Plains", 60f, "Stamina", "+{0} Max Stamina",
                Flat(CharacterStats.MaxStamina, "Totemic Endurance")));
            Register(new TrophyRite(TrophyId.Growth, "TrophyGrowth", "Tar Coating", "Growth", "Plains", 75f, "%", "{0}{1} Heavy / 40% Light Tar Proc Chance (8s)",
                null)); // Handled via DamagePipeline Phase 6 PostHitProcs
            Register(new TrophyRite(TrophyId.Lox, "TrophyLox", "Laborer's Mettle", "Lox", "Plains", 100f, "%", "+{0}{1} Tool Stamina Efficiency",
                Increased(CharacterStats.ToolStaminaEfficiency, "Laborer's Mettle")));
            Register(new TrophyRite(TrophyId.Vile, "TrophyBjornUndead", "Last Stand", "Vile", "Plains", 35f, "%", "Survive Lethal Blow with {0}{1} Max HP (10m CD)",
                null)); // Handled via DamagePipeline.OnLethalHit

            // Ocean
            Register(new TrophyRite(TrophyId.Serpent, "TrophySerpent", "Leviathan Glide", "Serpent", "Ocean", 100f, "%", "+{0}{1} Swim Speed",
                Increased(CharacterStats.SwimSpeed, "Leviathan Glide")));

            // Mistlands
            Register(new TrophyRite(TrophyId.Hare, "TrophyHare", "Bunny's Hop", "Hare", "Mistlands", 100f, "%", "+{0}{1} Jump Stamina Efficiency",
                Increased(CharacterStats.JumpEfficiency, "Bunny's Hop")));
            Register(new TrophyRite(TrophyId.Gjall, "TrophyGjall", "Gjall's Inferno", "Gjall", "Mistlands", 30f, "%", "+{0}{1} Elemental Magic Damage",
                Increased(CharacterStats.DamageElementalMagic, "Gjall's Inferno")));
            Register(new TrophyRite(TrophyId.Seeker, "TrophySeeker", "Razor Precision", "Seeker", "Mistlands", 25f, "%", "+{0}{1} Critical Strike Chance",
                (p, val, owner) => new[] {
                    StatModifier.Flat(CharacterStats.CriticalStrikeChance, val * 0.01f, "Razor Precision", null, owner)
                }));
            Register(new TrophyRite(TrophyId.SeekerSoldier, "TrophySeekerBrute", "Carapace Ward", "Seeker Soldier", "Mistlands", 1f, "s", "-80% Damage on First Hit ({0}{1} CD)",
                null)); // Handled via DamagePipeline.OnHitTaken
            Register(new TrophyRite(TrophyId.Tick, "TrophyTick", "Spare Vitality", "Tick", "Mistlands", 50f, "%", "+{0}{1} Blood Magic Health Efficiency",
                Increased(CharacterStats.AttackHealthEfficiency, "Spare Vitality")));
            Register(new TrophyRite(TrophyId.Dvergr, "TrophyDvergr", "Dvergr's Wisdom", "Dvergr", "Mistlands", 50f, "%", "+{0}{1} Skill Experience Gain",
                null)); // Handled directly in SE_Rested.m_raiseSkill / m_raiseSkillModifier

            // Ashlands
            Register(new TrophyRite(TrophyId.Asksvin, "TrophyAsksvin", "Cavalry Charge", "Asksvin", "Ashlands", 40f, "%", "+{0}{1} Mount Speed & Efficiency",
                (p, val, owner) => new[] {
                    StatModifier.Increased(CharacterStats.RidingSpeed, val * 0.01f, "Cavalry Charge", null, owner),
                    StatModifier.Increased(CharacterStats.RidingStaminaEfficiency, val * 0.01f, "Cavalry Charge", null, owner)
                }));
            Register(new TrophyRite(TrophyId.BonemawSerpent, "TrophyBonemawSerpent", "Sea Drake's Wake", "Bonemaw Serpent", "Ashlands", 45f, "%", "+{0}{1} Ship Sailing Speed",
                Increased(CharacterStats.ShipSailSpeed, "Sea Drake's Wake")));
            Register(new TrophyRite(TrophyId.FallenValkyrie, "TrophyFallenValkyrie", "Valkyrie's Aether", "Fallen Valkyrie", "Ashlands", 60f, "Eitr", "+{0} Max Eitr",
                Flat(CharacterStats.MaxEitr, "Valkyrie's Aether")));
            Register(new TrophyRite(TrophyId.CharredMarksman, "TrophyCharredArcher", "Snapdraw", "Charred Marksman", "Ashlands", 45f, "%", "+{0}{1} Draw & Reload Speed",
                (p, val, owner) => new[] {
                    StatModifier.Increased(CharacterStats.BowDrawSpeed, val * 0.01f, "Snapdraw", null, owner),
                    StatModifier.Increased(CharacterStats.CrossbowReloadSpeed, val * 0.01f, "Snapdraw", null, owner)
                }));
            Register(new TrophyRite(TrophyId.Morgen, "TrophyMorgen", "Morgen's Cull", "Morgen", "Ashlands", 50f, "%", "+{0}{1} Damage vs Low Health (<35% HP)",
                Increased(CharacterStats.DamageExecute, "Morgen's Cull")));
            Register(new TrophyRite(TrophyId.Volture, "TrophyVolture", "Wingshield", "Volture", "Ashlands", 75f, "%", "+{0}{1} Block Stamina Efficiency",
                Increased(CharacterStats.BlockStaminaEfficiency, "Wingshield")));
            Register(new TrophyRite(TrophyId.CharredWarlock, "TrophyCharredMage", "Cinder Shield", "Charred Warlock", "Ashlands", 20f, "%", "-{0}{1} Elemental Damage Taken",
                Resistance(CharacterStats.ResistanceElemental, "Cinder Shield")));
            Register(new TrophyRite(TrophyId.CharredWarrior, "TrophyCharredMelee", "Juggernaut", "Charred Warrior", "Ashlands", 25f, "%", "+{0}{1} 2-Handed Weapon Damage",
                Increased(CharacterStats.DamageTwoHanded, "Juggernaut")));

            // Deep North
            Register(new TrophyRite(TrophyId.Bear, "TrophyBjorn", "Ursine Ferocity", "Bear", "Deep North", 50f, "%", "+{0}{1} Berserk Damage (<35% HP)",
                Increased(CharacterStats.DamageBerserk, "Ursine Ferocity")));
            Register(new TrophyRite(TrophyId.Seal, "TrophySeal", "Aquatic Endurance", "Seal", "Deep North", 100f, "%", "+{0}{1} Swim Stamina Efficiency",
                Increased(CharacterStats.SwimEfficiency, "Aquatic Endurance")));
            Register(new TrophyRite(TrophyId.Moose, "TrophyMoose", "Draft Hauler", "Moose", "Deep North", 75f, "%", "+{0}{1} Cart Cargo Efficiency",
                Increased(CharacterStats.CartWeightEfficiency, "Draft Hauler")));
            Register(new TrophyRite(TrophyId.Mole, "TrophyMole", "Subterranean Greed", "Mole", "Deep North", 30f, "%", "+{0}{1} Mining Yield & Drop Rolls",
                Increased(CharacterStats.MiningYield, "Subterranean Greed")));
            Register(new TrophyRite(TrophyId.Barka, "TrophyBarka", "Frosted Hide", "Barka", "Deep North", 25f, "Armor", "+{0} Base Armor",
                Flat(CharacterStats.BodyArmor, "Frosted Hide")));
            Register(new TrophyRite(TrophyId.FrostBlob, "TrophyBlob_Frost", "Numbing Touch", "Frost Blob", "Deep North", 75f, "%", "{0}{1} Heavy / 40% Light Chill Attack Proc (8s)",
                null)); // Handled via DamagePipeline Phase 6 PostHitProcs
            Register(new TrophyRite(TrophyId.LavaBlob, "TrophyBlob_Lava", "Furnace Core", "Lava Blob", "Deep North", 25f, "%", "+{0}{1} Cooking Extra Yield",
                Increased(CharacterStats.CookingYield, "Furnace Core")));
            Register(new TrophyRite(TrophyId.JotunnWarrior, "TrophyJotunWarrior", "Titan's Swat", "Jötunn Warrior", "Deep North", 60f, "%", "+{0}{1} Knockback Force Dealt",
                Increased(CharacterStats.KnockbackDealt, "Titan's Swat")));
            Register(new TrophyRite(TrophyId.JotunnWitch, "TrophyJotunWitch", "Rime Weaver", "Jötunn Witch", "Deep North", 35f, "%", "+{0}{1} Eitr Spell Efficiency",
                Increased(CharacterStats.AttackEitrEfficiency, "Rime Weaver")));

            // Hildir's Request (Wildcards)
            Register(new TrophyRite(TrophyId.Brenna, "TrophySkeletonHildir", "Brenna's Pyre", "Brenna", "Hildir's Request", 35f, "%", "Crits/Sneak Hits trigger a Fireball Blast ({0}{1} AoE Fire)",
                null)); // Handled via DamagePipeline.OnCriticalHit
            Register(new TrophyRite(TrophyId.Geirrhafa, "TrophyCultist_Hildir", "Mountain Prowler", "Geirrhafa", "Hildir's Request", 35f, "%", "+{0}{1} Dodge Stamina Efficiency",
                Increased(CharacterStats.DodgeEfficiency, "Mountain Prowler")));
            Register(new TrophyRite(TrophyId.Zil, "TrophyGoblinBruteBrosShaman", "Shamanic Aegis", "Zil", "Hildir's Request", 12f, "%", "+{0}{1} Physical & Elemental Damage Reduction",
                (p, val, owner) => new[] {
                    StatModifier.Multiplier(CharacterStats.ResistancePhysical, Math.Max(0f, 1f - val * 0.01f), "Shamanic Aegis", null, owner),
                    StatModifier.Multiplier(CharacterStats.ResistanceElemental, Math.Max(0f, 1f - val * 0.01f), "Shamanic Aegis", null, owner)
                }));
            Register(new TrophyRite(TrophyId.Steil, "TrophyGoblinBruteBrosBrute", "Unstoppable Poise", "Steil", "Hildir's Request", 75f, "%", "+{0}{1} Stagger Poise Threshold",
                Increased(CharacterStats.StaggerResistance, "Unstoppable Poise")));
        }

        public static void Register(TrophyRite rite)
        {
            ById[rite.Id] = rite;
            ByPrefab[rite.PrefabName] = rite;
            foreach (string alt in rite.AlternatePrefabs)
            {
                if (!string.IsNullOrEmpty(alt))
                {
                    ByPrefab[alt] = rite;
                }
            }
        }

        public static bool TryGetRite(string prefabName, out TrophyRite rite)
        {
            return ByPrefab.TryGetValue(prefabName, out rite!);
        }

        public static bool TryGetRite(TrophyId id, out TrophyRite rite)
        {
            return ById.TryGetValue(id, out rite!);
        }

        public static IEnumerable<TrophyRite> GetAllRites() => ById.Values;
    }
}
