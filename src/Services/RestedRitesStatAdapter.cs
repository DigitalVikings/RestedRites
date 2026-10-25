using System;
using System.Collections.Generic;
using UnityEngine;
using VikingLib.Combat;
using VikingLib.Stats;
using VikingLib.UI;
using RestedRites.Configuration;
using RestedRites.Patches;
using RestedRites.Rites;

namespace RestedRites.Services
{
    public static class RestedRitesStatAdapter
    {
        public const string OwnerKey = "RestedRites";

        private static bool _initialized = false;
        private static bool s_bloodboundEmpowered = false;

        public static void Init()
        {
            if (_initialized) return;
            _initialized = true;

            // 1. Pipeline Phase 1: Attacker Modifiers
            DamagePipeline.RegisterHandler(DamagePhase.AttackerModifiers, 50, OnAttackerModifiers);

            // 2. Pipeline Defensive Hook: Carapace Ward & Slime Splash
            DamagePipeline.OnHitTaken += OnHitTaken;

            // 3. Pipeline Lethal Hit: Vile Last Stand Death Defiance
            DamagePipeline.OnLethalHit += OnLethalHit;

            // 4. Pipeline OnKill: Bloodbound 3pc Empower
            DamagePipeline.OnKill += OnKill;

            // 5. Pipeline OnCriticalHit: Brenna Fireball Blast
            DamagePipeline.OnCriticalHit += OnCriticalHit;

            // 6. Pipeline Phase 6: PostHitProcs (Fenring, Tar, Chill)
            DamagePipeline.RegisterHandler(DamagePhase.PostHitProcs, 50, OnPostHitProcs);

            // 7. Compendium: Register custom 'Rested Rites' category in Hugin's Compendium
            CharacterStatsGlossaryPanel.RegisterCustomCategory("Rested Rites", ProvideGlossaryCategory, priority: 50);
        }

        public static void SyncPlayer(Player? player)
        {
            if (player == null || player != Player.m_localPlayer || !ModConfig.ModEnabled.Value)
            {
                if (player != null)
                {
                    CharacterStatManager.RemoveModifiersByOwner(player, OwnerKey);
                }
                return;
            }

            var seMan = player.GetSEMan();
            if (seMan == null)
            {
                CharacterStatManager.RemoveModifiersByOwner(player, OwnerKey);
                return;
            }

            var rested = seMan.GetStatusEffect(StringExtensionMethods.GetStableHashCode("Rested")) as SE_Rested;
            if (rested == null)
            {
                CharacterStatManager.RemoveModifiersByOwner(player, OwnerKey);
                return;
            }

            int comfort = SERestedPatch.GetComfortLevel(rested);
            var activeEntries = AttunementManager.GetActiveEntries(player, comfort);
            var modifiers = new List<StatModifier>(64);

            // 1. Evaluate Active Trophy Rites
            for (int i = 0; i < activeEntries.Count; i++)
            {
                var entry = activeEntries[i];
                if (RiteDatabase.TryGetRite(entry.PrefabName, out var rite))
                {
                    foreach (var mod in rite.CreateModifiers(player, comfort, i, OwnerKey))
                    {
                        modifiers.Add(mod);
                    }
                }
            }

            // 2. Evaluate Active Set Bonuses
            var activeSetCounts = AttunementManager.GetActiveSetCounts(player);
            foreach (var kvp in activeSetCounts)
            {
                if (SetDatabase.TryGetSet(kvp.Key, out var set))
                {
                    foreach (var mod in set.CreateModifiers(player, kvp.Value, OwnerKey))
                    {
                        modifiers.Add(mod);
                    }
                }
            }

            // 3. Atomically Replace All RestedRites Modifiers
            CharacterStatManager.ReplaceModifiersByOwner(player, OwnerKey, modifiers);
        }

        private static void OnAttackerModifiers(DamageContext ctx)
        {
            if (ctx.Attacker is not Player player || player != Player.m_localPlayer) return;

            // The Bloodbound 3-Piece: 2x Damage on Next Attack after Kill
            if (s_bloodboundEmpowered && AttunementManager.HasActiveSetBonus(player, SetId.TheBloodbound, 3))
            {
                s_bloodboundEmpowered = false;
                ctx.AddMultiplier(2.0f, "The Bloodbound (Empowered)");
                FeedbackManager.TriggerBloodboundStrike(ctx.Hit.m_point);
            }

            // The Plains Tribe 4-Piece: +15% Fire damage from Physical (using first-class VikingLib API)
            if (AttunementManager.HasActiveSetBonus(player, SetId.ThePlainsTribe, 4))
            {
                ctx.AddExtraDamageFromPhysical(DamageType.Fire, 0.15f, "The Plains Tribe (4pc)");
            }

            // The Charred Duo 2-Piece: +25% Damage with Elemental Arrows
            if (ctx.Hit.m_ranged && AttunementManager.HasActiveSetBonus(player, SetId.TheCharredDuo, 2))
            {
                if (ctx.GetTotalElementalDamage() > 0.01f || ctx.GetDamage(DamageType.Poison) > 0.01f || ctx.GetDamage(DamageType.Spirit) > 0.01f)
                {
                    ctx.AddIncreasedPercent(0.25f, "The Charred Duo");
                }
            }
        }

        private static void OnHitTaken(DamageContext ctx)
        {
            if (ctx.Target is not Player player || player != Player.m_localPlayer) return;

            // Seeker Soldier (Carapace Ward): 80% Damage Reduction on First Hit (managed via ProcCooldownManager)
            if (AttunementManager.TryGetActivePower(player, TrophyId.SeekerSoldier, out float wardPower))
            {
                float cd = wardPower > 0.01f ? (75f / wardPower) : 75f;
                if (ProcCooldownManager.TryTrigger(player, "RestedRites_CarapaceWard", cd, persist: false))
                {
                    float rawIncoming = ctx.GetTotalDamage();

                    // Apply 80% physical mitigation
                    ctx.AddMultiplier(0.20f, "Carapace Ward", DamageType.Blunt);
                    ctx.AddMultiplier(0.20f, "Carapace Ward", DamageType.Slash);
                    ctx.AddMultiplier(0.20f, "Carapace Ward", DamageType.Pierce);
                    FeedbackManager.TriggerCarapaceWard(player, ctx.Hit.m_point);
                    player.Message(MessageHud.MessageType.TopLeft, $"Carapace Ward absorbed 80% damage! ({cd:F0}s CD)");

                    // The Hive Chitin 3-Piece: Carapace Counter (Reflect 50% damage back via DamagePipeline.ReflectDamage)
                    if (ctx.Attacker != null && AttunementManager.HasActiveSetBonus(player, SetId.TheHiveChitin, 3))
                    {
                        if (DamagePipeline.ReflectDamage(player, ctx.Attacker, ctx, 0.50f, DamageType.Blunt, "Carapace Counter"))
                        {
                            FeedbackManager.TriggerCarapaceCounter(ctx.Attacker, rawIncoming * 0.50f);
                            player.Message(MessageHud.MessageType.TopLeft, "<color=#00FFFF><b>Carapace Counter: Reflected 50% damage!</b></color>");
                        }
                    }
                }
            }

            // The Gelatinous Horde 3-Piece: Slime Splash (Chills Melee Attackers via ApplyOnHitStatus)
            if (ctx.Attacker != null && !ctx.Hit.m_ranged && AttunementManager.HasActiveSetBonus(player, SetId.TheGelatinousHorde, 3))
            {
                DamagePipeline.ApplyOnHitStatus(ctx.Attacker, "Frost", 0f, 4f, player);
            }
        }

        private static void OnLethalHit(DamageContext ctx)
        {
            if (ctx.Target is not Player player || player != Player.m_localPlayer) return;

            // Vile (Last Stand): Survive lethal damage & heal (persisted across relogs via ProcCooldownManager)
            if (AttunementManager.TryGetActivePower(player, TrophyId.Vile, out float vileBonus))
            {
                float cd = AttunementManager.HasActiveSetBonus(player, SetId.TheDraugrRevenants, 3) ? 300f : 600f;
                if (ProcCooldownManager.TryTrigger(player, "RestedRites_LastStand", cd, persist: true))
                {
                    // Clamps lethal blow to 1 HP
                    ctx.DefyDeath();

                    float healAmount = player.GetMaxHealth() * (vileBonus / 100f);
                    player.Heal(healAmount, true);
                    FeedbackManager.TriggerLastStand(player, healAmount);

                    if (AttunementManager.HasActiveSetBonus(player, SetId.TheDraugrRevenants, 3))
                    {
                        player.AddStamina(player.GetMaxStamina());
                        player.Message(MessageHud.MessageType.Center, "<color=#00FFFF><b>Last Stand: Defied Death & Restored Stamina!</b></color>");
                    }
                    else
                    {
                        player.Message(MessageHud.MessageType.Center, "<color=#FFD700><b>Last Stand: Defied Death!</b></color>");
                    }
                }
            }
        }

        private static void OnKill(Character attacker, Character victim, DamageContext ctx)
        {
            if (attacker is not Player player || player != Player.m_localPlayer) return;

            // The Bloodbound 3-Piece: Empower Next Attack
            if (AttunementManager.HasActiveSetBonus(player, SetId.TheBloodbound, 3))
            {
                s_bloodboundEmpowered = true;
                player.Message(MessageHud.MessageType.TopLeft, "<color=#FF4500><b>Bloodbound: Next attack deals 2x damage!</b></color>");
            }
        }

        private static void OnCriticalHit(Character attacker, Character victim, DamageContext ctx)
        {
            if (attacker is not Player player || player != Player.m_localPlayer) return;

            // Brenna (Brenna's Pyre): Crits & Sneak Hits trigger an AoE Fireball Blast
            if (AttunementManager.TryGetActivePower(player, TrophyId.Brenna, out float brennaPower))
            {
                float totalDamage = ctx.GetTotalDamage();
                float aoeFire = totalDamage * (brennaPower / 100f);
                Vector3 burstPoint = ctx.Hit.m_point != Vector3.zero
                    ? ctx.Hit.m_point
                    : (victim != null ? victim.GetCenterPoint() : player.transform.position);

                FeedbackManager.TriggerBrennaFireburst(burstPoint, aoeFire, player);
            }
        }

        private static void OnPostHitProcs(DamageContext ctx)
        {
            if (ctx.Attacker is not Player attacker || attacker != Player.m_localPlayer) return;

            var target = ctx.Target;

            // Fenring (Predator's Rush): +Speed on Hit
            if (AttunementManager.TryGetActivePower(attacker, TrophyId.Fenring, out _))
            {
                FeedbackManager.TriggerSpeedBoost(attacker);
            }

            // Growth (Tar Coating): Heavy/ranged or light proc chance via ApplyOnHitStatus
            if (target != null && AttunementManager.TryGetActivePower(attacker, TrophyId.Growth, out float tarPower))
            {
                bool isRanged = ctx.Hit.m_ranged;
                float chance = isRanged ? tarPower : 40f;
                if (UnityEngine.Random.Range(0f, 100f) < chance)
                {
                    DamagePipeline.ApplyOnHitStatus(target, "Tarred", 0f, 8f, attacker);
                    FeedbackManager.TriggerTarProc(target);
                }
            }

            // Frost Blob (Numbing Touch): Heavy/ranged or light proc chance via ApplyOnHitStatus
            if (target != null && AttunementManager.TryGetActivePower(attacker, TrophyId.FrostBlob, out float frostPower))
            {
                bool isRanged = ctx.Hit.m_ranged;
                float chance = isRanged ? frostPower : 40f;
                if (UnityEngine.Random.Range(0f, 100f) < chance)
                {
                    DamagePipeline.ApplyOnHitStatus(target, "Frost", 0f, 8f, attacker);
                    FeedbackManager.TriggerFrostProc(target);
                }
            }
        }

        /// <summary>
        /// Provides structured rows for the 'Rested Rites' category inside Hugin's Compendium (Character Stats panel).
        /// Automatically sparse-pruned by VikingLib when empty.
        /// </summary>
        private static IEnumerable<(string Label, string FormattedValue)>? ProvideGlossaryCategory(Player player)
        {
            if (player == null || !ModConfig.ModEnabled.Value) return null;

            var attunedEntries = AttunementManager.GetAttunedEntries(player);
            if (attunedEntries.Count == 0) return null;

            var rested = player.GetSEMan()?.GetStatusEffect(StringExtensionMethods.GetStableHashCode("Rested")) as SE_Rested;
            bool isRested = rested != null;
            int comfort = isRested ? SERestedPatch.GetComfortLevel(rested) : SERestedPatch.GetPlayerComfort(player);
            int activeSlots = PowerCalculator.GetActiveSlots(comfort);
            int activeCount = isRested ? Math.Min(attunedEntries.Count, activeSlots) : 0;

            var rows = new List<(string Label, string FormattedValue)>();

            string statusVal = isRested
                ? $"{activeCount}/{activeSlots} Active (Comfort {comfort} • {ModConfig.ProgressionType.Value} Model)"
                : $"Inactive (Requires Rested • Comfort {comfort})";

            rows.Add(("Attunement Status", statusVal));

            for (int i = 0; i < attunedEntries.Count; i++)
            {
                var entry = attunedEntries[i];
                if (!RiteDatabase.TryGetRite(entry.PrefabName, out TrophyRite rite)) continue;

                bool isActive = isRested && i < activeCount;
                float slotPct = PowerCalculator.GetSlotPercent(i, comfort);
                string slotName = i == 0 ? "Anchor" : $"Slot {i + 1}";
                string desc = isActive ? rite.GetFormattedDescription(comfort, i) : rite.GetBaseDescription();
                string tag = isActive ? $"{slotName} ({slotPct:0.#}%)" : (isRested ? "Requires Higher Comfort" : "Requires Rested");

                rows.Add(($"• {rite.DisplayName} ({rite.CreatureName})", $"{desc} [{tag}]"));
            }

            if (isRested)
            {
                var activeSetCounts = AttunementManager.GetActiveSetCounts(player);
                foreach (var kvp in activeSetCounts)
                {
                    if (SetDatabase.TryGetSet(kvp.Key, out TrophySet set))
                    {
                        int pieces = kvp.Value;
                        var bonuses = new List<string>();
                        if (pieces >= 2 && set.TwoPieceBonus != null) bonuses.Add($"(2) {set.TwoPieceBonus}");
                        if (pieces >= 3 && set.ThreePieceBonus != null) bonuses.Add($"(3) {set.ThreePieceBonus}");
                        if (pieces >= 4 && set.FourPieceBonus != null) bonuses.Add($"(4) {set.FourPieceBonus}");

                        if (bonuses.Count > 0)
                        {
                            rows.Add(($"• Set: {set.Name} ({pieces}/{set.Members.Length})", string.Join(" | ", bonuses)));
                        }
                    }
                }
            }

            return rows;
        }
    }
}
