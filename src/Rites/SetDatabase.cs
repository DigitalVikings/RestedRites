using System;
using System.Collections.Generic;
using UnityEngine;
using VikingLib.Stats;

namespace RestedRites.Rites
{
    public static class SetDatabase
    {
        private static readonly Dictionary<SetId, TrophySet> SetsById = new();
        private static readonly Dictionary<TrophyId, TrophySet> SetByTrophy = new();

        public static bool IsNearTree(Vector3 pos, float radius)
        {
            var colliders = Physics.OverlapSphere(pos, radius);
            for (int i = 0; i < colliders.Length; i++)
            {
                var col = colliders[i];
                if (col != null && (col.GetComponentInParent<TreeBase>() != null || col.GetComponentInParent<TreeLog>() != null))
                {
                    return true;
                }
            }
            return false;
        }

        static SetDatabase()
        {
            // 4-Piece Sets
            Register(new TrophySet(
                SetId.PioneersOfTheWilds,
                "Pioneers of the Wilds",
                new[] { TrophyId.Boar, TrophyId.Deer, TrophyId.Neck, TrophyId.Hare },
                twoPieceBonus: "+15% Sprint Stamina Efficiency when carry weight is below 50%",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.SprintEfficiency, 0.15f, "Pioneers of the Wilds (2pc)",
                        c => c is Player player && player.GetInventory().GetTotalWeight() < player.GetMaxCarryWeight() * 0.5f, owner)
                },
                threePieceBonus: "+5% Double gather & drop roll yield",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.ForagingYield, 0.05f, "Pioneers of the Wilds (3pc)", null, owner)
                },
                fourPieceBonus: "+25% Movement Speed",
                fourPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.JogSpeed, 0.25f, "Pioneers of the Wilds (4pc)", null, owner),
                    StatModifier.Increased(CharacterStats.RunSpeed, 0.25f, "Pioneers of the Wilds (4pc)", null, owner)
                }
            ));

            Register(new TrophySet(
                SetId.TheRestlessBarrow,
                "The Restless Barrow",
                new[] { TrophyId.Skeleton, TrophyId.RancidRemains, TrophyId.Ghost, TrophyId.Wraith },
                twoPieceBonus: "-35% Piercing damage taken",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Multiplier(CharacterStats.ResistancePiercing, 0.65f, "The Restless Barrow (2pc)", null, owner)
                },
                threePieceBonus: "+1.0x Sneak attack damage multiplier",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.DamageSneak, 1.0f, "The Restless Barrow (3pc)", null, owner)
                },
                fourPieceBonus: "+20% Sneak movement speed & +35% Sneak Stamina Efficiency",
                fourPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.CrouchSpeed, 0.20f, "The Restless Barrow (4pc)", null, owner),
                    StatModifier.Increased(CharacterStats.SneakEfficiency, 0.35f, "The Restless Barrow (4pc)", null, owner)
                }
            ));

            Register(new TrophySet(
                SetId.TheEarthshakers,
                "The Earthshakers",
                new[] { TrophyId.Troll, TrophyId.StoneGolem, TrophyId.Morgen, TrophyId.JotunnWarrior },
                twoPieceBonus: "+40% Pickaxe damage to rocks, ore deposits, and barriers",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.DamagePickaxe, 0.40f, "The Earthshakers (2pc)", null, owner)
                },
                threePieceBonus: "+25% Knockback force dealt & stagger power",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.KnockbackDealt, 0.25f, "The Earthshakers (3pc)", null, owner),
                    StatModifier.Increased(CharacterStats.StaggerDamage, 0.25f, "The Earthshakers (3pc)", null, owner)
                },
                fourPieceBonus: "+35% Multi-target cleave damage efficiency",
                fourPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.CleaveEfficiency, 0.35f, "The Earthshakers (4pc)", null, owner)
                }
            ));

            Register(new TrophySet(
                SetId.ThePlainsTribe,
                "The Plains Tribe",
                new[] { TrophyId.Fuling, TrophyId.Surtling, TrophyId.Wolf, TrophyId.Growth },
                twoPieceBonus: "+25% Damage vs tarred, slowed, or staggered enemies",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.DamageAgainstAfflicted, 0.25f, "The Plains Tribe (2pc)", null, owner)
                },
                threePieceBonus: "+25 Max Stamina & +20% Dodge Stamina Efficiency",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Flat(CharacterStats.MaxStamina, 25f, "The Plains Tribe (3pc)", null, owner),
                    StatModifier.Increased(CharacterStats.DodgeEfficiency, 0.20f, "The Plains Tribe (3pc)", null, owner)
                },
                fourPieceBonus: "Adds +15% Fire damage to all physical damage dealt"
                // fourPieceModifiers handled via Phase 1 pipeline handler
            ));

            Register(new TrophySet(
                SetId.BeastmastersOfTheHearth,
                "Beastmasters of the Hearth",
                new[] { TrophyId.Lox, TrophyId.Asksvin, TrophyId.Bear, TrophyId.Moose },
                twoPieceBonus: "+75% Stamina regeneration while standing completely still",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.StaminaRegen, 0.75f, "Beastmasters of the Hearth (2pc)",
                        c => c.GetVelocity().sqrMagnitude < 0.05f, owner)
                },
                threePieceBonus: "+75% Cart Cargo Efficiency & +25% Tool Stamina Efficiency",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.CartWeightEfficiency, 0.75f, "Beastmasters of the Hearth (3pc)", null, owner),
                    StatModifier.Increased(CharacterStats.ToolStaminaEfficiency, 0.25f, "Beastmasters of the Hearth (3pc)", null, owner)
                },
                fourPieceBonus: "+50% Mount Stamina Efficiency and +20% Mount Speed",
                fourPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.RidingStaminaEfficiency, 0.50f, "Beastmasters of the Hearth (4pc)", null, owner),
                    StatModifier.Increased(CharacterStats.RidingSpeed, 0.20f, "Beastmasters of the Hearth (4pc)", null, owner)
                }
            ));

            Register(new TrophySet(
                SetId.WeaversOfEitr,
                "Weavers of Eitr",
                new[] { TrophyId.Cultist, TrophyId.Gjall, TrophyId.FallenValkyrie, TrophyId.JotunnWitch },
                twoPieceBonus: "+15% Eitr regeneration & +10% Spell Eitr Efficiency",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.EitrRegen, 0.15f, "Weavers of Eitr (2pc)", null, owner),
                    StatModifier.Increased(CharacterStats.AttackEitrEfficiency, 0.10f, "Weavers of Eitr (2pc)", null, owner)
                },
                threePieceBonus: "+30 Max Eitr",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Flat(CharacterStats.MaxEitr, 30f, "Weavers of Eitr (3pc)", null, owner)
                },
                fourPieceBonus: "+25% Spell Eitr Efficiency",
                fourPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.AttackEitrEfficiency, 0.25f, "Weavers of Eitr (4pc)", null, owner)
                }
            ));

            // 3-Piece Sets
            Register(new TrophySet(
                SetId.ChildrenOfTheForest,
                "Children of the Forest",
                new[] { TrophyId.Greydwarf, TrophyId.GreydwarfBrute, TrophyId.GreydwarfShaman },
                twoPieceBonus: "+20% Wood yield from trees",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.WoodcuttingYield, 0.20f, "Children of the Forest (2pc)", null, owner)
                },
                threePieceBonus: "+20% Health regeneration within 20m of any tree",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.HealthRegen, 0.20f, "Children of the Forest (3pc)",
                        c => IsNearTree(c.transform.position, 20f), owner)
                }
            ));

            Register(new TrophySet(
                SetId.TheDraugrRevenants,
                "The Draugr Revenants",
                new[] { TrophyId.Draugr, TrophyId.DraugrElite, TrophyId.Vile },
                twoPieceBonus: "+40% Stagger buildup on sneak attacks",
                threePieceBonus: "50% Cooldown reduction on Last Stand (down to 5m) & restores full stamina"
                // Handled via RestedRitesStatAdapter pipeline hooks
            ));

            Register(new TrophySet(
                SetId.TheGelatinousHorde,
                "The Gelatinous Horde",
                new[] { TrophyId.Blob, TrophyId.FrostBlob, TrophyId.LavaBlob },
                twoPieceBonus: "+25% Jump height & -50% fall damage",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.JumpHeight, 0.25f, "The Gelatinous Horde (2pc)", null, owner),
                    StatModifier.Multiplier(CharacterStats.ResistanceFall, 0.50f, "The Gelatinous Horde (2pc)", null, owner)
                },
                threePieceBonus: "Slime Splash: Melee hits suffered emit slime chilling attackers"
                // Handled via DamagePipeline.OnHitTaken
            ));

            Register(new TrophySet(
                SetId.TheBloodbound,
                "The Bloodbound",
                new[] { TrophyId.Bat, TrophyId.Leech, TrophyId.CharredWarrior },
                twoPieceBonus: "+20 Stamina restored on kill",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Flat(CharacterStats.StaminaOnKill, 20f, "The Bloodbound (2pc)", null, owner)
                },
                threePieceBonus: "2x Damage on your next attack after killing an enemy"
                // Handled via DamagePipeline.OnKill empower
            ));

            Register(new TrophySet(
                SetId.TheHiveChitin,
                "The Hive Chitin",
                new[] { TrophyId.Seeker, TrophyId.SeekerSoldier, TrophyId.Tick },
                twoPieceBonus: "+15 Base Armor",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Flat(CharacterStats.BodyArmor, 15f, "The Hive Chitin (2pc)", null, owner)
                },
                threePieceBonus: "Carapace Counter: Reflect 50% damage when Carapace Ward triggers"
                // Handled via DamagePipeline.OnHitTaken
            ));

            Register(new TrophySet(
                SetId.UndergroundDelvers,
                "Underground Delvers",
                new[] { TrophyId.Dvergr, TrophyId.Mole, TrophyId.Barka },
                twoPieceBonus: "+100% Tool and Weapon Durability Efficiency",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.DurabilityEfficiency, 1.00f, "Underground Delvers (2pc)", null, owner)
                },
                threePieceBonus: "+20% Mining yield & +10 Armor in caves and crypts",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.MiningYield, 0.20f, "Underground Delvers (3pc)", null, owner),
                    StatModifier.Flat(CharacterStats.BodyArmor, 10f, "Underground Delvers (3pc)", c => c is Player player && player.InInterior(), owner)
                }
            ));

            Register(new TrophySet(
                SetId.TidecallersOfTheAbyss,
                "Tidecallers of the Abyss",
                new[] { TrophyId.Serpent, TrophyId.BonemawSerpent, TrophyId.Seal },
                twoPieceBonus: "+50% Swim Stamina Efficiency & +30% damage reduction in water",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.SwimEfficiency, 0.50f, "Tidecallers of the Abyss (2pc)", null, owner),
                    StatModifier.Multiplier(CharacterStats.ResistancePhysical, 0.70f, "Tidecallers of the Abyss (2pc)", c => c.InWater(), owner),
                    StatModifier.Multiplier(CharacterStats.ResistanceElemental, 0.70f, "Tidecallers of the Abyss (2pc)", c => c.InWater(), owner)
                },
                threePieceBonus: "+15% Ship sailing speed",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.ShipSailSpeed, 0.15f, "Tidecallers of the Abyss (3pc)", null, owner)
                }
            ));

            Register(new TrophySet(
                SetId.WingsOfPrey,
                "Wings of Prey",
                new[] { TrophyId.Drake, TrophyId.Deathsquito, TrophyId.Volture },
                twoPieceBonus: "+15% Piercing damage & +20% Shield Block Stamina Efficiency",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.DamagePiercing, 0.15f, "Wings of Prey (2pc)", null, owner),
                    StatModifier.Increased(CharacterStats.BlockStaminaEfficiency, 0.20f, "Wings of Prey (2pc)", null, owner),
                    StatModifier.Increased(CharacterStats.BowDrawStaminaEfficiency, 0.20f, "Wings of Prey (2pc)", null, owner)
                },
                threePieceBonus: "Dive Bomb: Attacks from mid-air or elevation deal +35% damage",
                threePieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.DamageWhileAirborne, 0.35f, "Wings of Prey (3pc)", null, owner)
                }
            ));

            // 2-Piece Sets
            Register(new TrophySet(
                SetId.TheGoblinChieftains,
                "The Goblin Chieftains",
                new[] { TrophyId.FulingBerserker, TrophyId.FulingShaman },
                twoPieceBonus: "+15 Stamina restored upon staggering an enemy",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Flat(CharacterStats.StaminaOnCauseStagger, 15f, "The Goblin Chieftains (2pc)", null, owner)
                }
            ));

            Register(new TrophySet(
                SetId.TheFenrisKin,
                "The Fenris Kin",
                new[] { TrophyId.Fenring, TrophyId.Ulv },
                twoPieceBonus: "+30% Damage on the 3rd hit of weapon combo chains",
                twoPieceModifiers: (p, owner) => new[] {
                    StatModifier.Increased(CharacterStats.DamageComboFinisher, 0.30f, "The Fenris Kin (2pc)", null, owner)
                }
            ));

            Register(new TrophySet(
                SetId.TheCharredDuo,
                "The Charred Duo",
                new[] { TrophyId.CharredMarksman, TrophyId.CharredWarlock },
                twoPieceBonus: "+25% Damage dealt with elemental arrows"
                // Handled via Phase 1 pipeline handler
            ));
        }

        private static void Register(TrophySet set)
        {
            SetsById[set.Id] = set;
            foreach (var trophy in set.Members)
            {
                SetByTrophy[trophy] = set;
            }
        }

        public static IEnumerable<TrophySet> GetAllSets() => SetsById.Values;

        public static bool TryGetSet(SetId id, out TrophySet set)
        {
            return SetsById.TryGetValue(id, out set!);
        }

        public static bool TryGetSetForTrophy(TrophyId trophy, out TrophySet set)
        {
            return SetByTrophy.TryGetValue(trophy, out set!);
        }
    }
}
