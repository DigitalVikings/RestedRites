using System;
using System.Collections.Generic;
using HarmonyLib;
using RestedRites.Configuration;
using RestedRites.Rites;
using RestedRites.Services;
using UnityEngine;

namespace RestedRites.Patches
{
    [HarmonyPatch(typeof(SE_Rested))]
    public static class SERestedPatch
    {
        private static readonly AccessTools.FieldRef<StatusEffect, float> StatusEffectTimeRef =
            AccessTools.FieldRefAccess<StatusEffect, float>("m_time");

        private static readonly Dictionary<SE_Rested, int> RestedComfortMap = new();
        private static readonly Dictionary<SE_Rested, bool> AbominationActiveMap = new();

        public static bool HasAbominationRite(Player? player, int comfortLevel)
        {
            if (player == null) return false;
            var activeEntries = AttunementManager.GetActiveEntries(player, comfortLevel);
            for (int i = 0; i < activeEntries.Count; i++)
            {
                if (RiteDatabase.TryGetRite(activeEntries[i].PrefabName, out var rite) && rite.Id == TrophyId.Abomination)
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetComfortLevel(SE_Rested? rested)
        {
            if (rested != null && RestedComfortMap.TryGetValue(rested, out int cached) && cached > 0)
            {
                return cached;
            }

            if (rested?.m_character is Player player)
            {
                int c = SE_Rested.CalculateComfortLevel(player.InShelter(), player.transform.position);
                if (c > 1)
                {
                    RestedComfortMap[rested] = c;
                }
                return c;
            }

            if (Player.m_localPlayer != null)
            {
                return SE_Rested.CalculateComfortLevel(Player.m_localPlayer.InShelter(), Player.m_localPlayer.transform.position);
            }

            return 1;
        }

        public static float CalculateRestedDuration(int comfortLevel, bool hasAbomination = false)
        {
            int c = Math.Max(1, comfortLevel);
            if (!ModConfig.EnableCustomDuration.Value)
            {
                return 480f + (c - 1) * 60f;
            }

            float baseSeconds = ModConfig.BaseDurationMinutes.Value * 60f;
            float perComfort = hasAbomination ? 60f : ModConfig.DurationPerComfortSeconds.Value;
            return baseSeconds + (c - 1) * perComfort;
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateTTL")]
        public static bool UpdateTTL_Prefix(SE_Rested __instance)
        {
            if (!ModConfig.ModEnabled.Value || __instance == null)
            {
                return true;
            }

            int liveComfort = 1;
            if (__instance.m_character is Player player)
            {
                liveComfort = Math.Max(player.GetComfortLevel(), SE_Rested.CalculateComfortLevel(player.InShelter(), player.transform.position));
            }
            else if (Player.m_localPlayer != null)
            {
                liveComfort = Math.Max(Player.m_localPlayer.GetComfortLevel(), SE_Rested.CalculateComfortLevel(Player.m_localPlayer.InShelter(), Player.m_localPlayer.transform.position));
            }

            // Maintain highest comfort achieved while this Rested buff is active.
            // Walking outside into lower comfort must never truncate or downgrade an already earned Rested buff.
            int currentComfort = 1;
            if (RestedComfortMap.TryGetValue(__instance, out int cachedComfort) && cachedComfort > 0)
            {
                currentComfort = cachedComfort;
            }

            if (liveComfort > currentComfort)
            {
                currentComfort = liveComfort;
                RestedComfortMap[__instance] = currentComfort;
            }
            else if (cachedComfort == 0 && liveComfort > 0)
            {
                currentComfort = liveComfort;
                RestedComfortMap[__instance] = currentComfort;
            }

            var targetPlayer = __instance.m_character as Player ?? Player.m_localPlayer;
            bool hasAbomination = HasAbominationRite(targetPlayer, currentComfort);
            float newTtl = CalculateRestedDuration(currentComfort, hasAbomination);

            float remaining = Mathf.Max(0f, __instance.GetRemaningTime());
            // Upgrade m_ttl if the new comfort produces a duration longer than remaining time,
            // or if the buff is being freshly initialized / reset.
            if (newTtl > remaining || __instance.m_ttl <= 0f)
            {
                __instance.m_ttl = newTtl;
                StatusEffectTimeRef(__instance) = 0f;
            }

            AbominationActiveMap[__instance] = hasAbomination;

            ApplyCustomModifiers(__instance, targetPlayer);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SE_Rested.ResetTime))]
        public static void ResetTime_Postfix(SE_Rested __instance)
        {
            ApplyCustomModifiers(__instance, __instance.m_character as Player ?? Player.m_localPlayer);
        }

        private static readonly AccessTools.FieldRef<StatusEffect, GameObject[]> StatusEffectStartEffectInstancesRef =
            AccessTools.FieldRefAccess<StatusEffect, GameObject[]>("m_startEffectInstances");

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SE_Rested.Setup))]
        public static void Setup_Postfix(SE_Rested __instance, Character character)
        {
            ApplyCustomModifiers(__instance, character as Player ?? Player.m_localPlayer);

            // Eliminate lingering purple mist particles on gaining the Rested buff
            var instances = StatusEffectStartEffectInstancesRef(__instance);
            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    if (instances[i] != null)
                    {
                        UnityEngine.Object.Destroy(instances[i]);
                    }
                }
                StatusEffectStartEffectInstancesRef(__instance) = Array.Empty<GameObject>();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SE_Rested.UpdateStatusEffect), typeof(float))]
        public static void UpdateStatusEffect_Postfix(SE_Rested __instance)
        {
            ApplyCustomModifiers(__instance, __instance.m_character as Player ?? Player.m_localPlayer);
        }

        public static void OnRestedStop(SE_Rested rested)
        {
            if (rested != null)
            {
                RestedComfortMap.Remove(rested);
                AbominationActiveMap.Remove(rested);
            }

            if (Player.m_localPlayer != null)
            {
                RestedRitesStatAdapter.SyncPlayer(Player.m_localPlayer);
            }
        }

        public static int GetPlayerComfort(Player? player)
        {
            if (player == null) return 1;

            var rested = player.GetSEMan()?.GetStatusEffect(StringExtensionMethods.GetStableHashCode("Rested")) as SE_Rested;
            if (rested != null)
            {
                return GetComfortLevel(rested);
            }

            return SE_Rested.CalculateComfortLevel(player.InShelter(), player.transform.position);
        }

        public static void ReapplyCustomModifiers(Player? player)
        {
            if (player == null) return;
            var rested = player.GetSEMan()?.GetStatusEffect(StringExtensionMethods.GetStableHashCode("Rested")) as SE_Rested;
            if (rested != null)
            {
                ApplyCustomModifiers(rested, player);
            }
            else
            {
                RestedRitesStatAdapter.SyncPlayer(player);
            }
        }

        public static void ApplyCustomModifiers(SE_Rested rested, Player? player)
        {
            if (!ModConfig.ModEnabled.Value || rested == null)
            {
                return;
            }

            // 1. Synchronize VikingLib 3-Tier Stats
            RestedRitesStatAdapter.SyncPlayer(player);

            if (player == null) return;

            int playerComfort = GetComfortLevel(rested);
            var activeEntries = AttunementManager.GetActiveEntries(player, playerComfort);

            // 2. Baseline Rested Buff Regeneration & Skill XP Isolation
            if (ModConfig.EnableBaselineRestedBuff.Value)
            {
                rested.m_healthRegenMultiplier = ModConfig.BaselineHealthRegen.Value;
                rested.m_staminaRegenMultiplier = ModConfig.BaselineStaminaRegen.Value;
                rested.m_eitrRegenMultiplier = ModConfig.BaselineEitrRegen.Value;
            }
            else
            {
                rested.m_healthRegenMultiplier = 1.0f;
                rested.m_staminaRegenMultiplier = 1.0f;
                rested.m_eitrRegenMultiplier = 1.0f;
            }

            // Dvergr's Wisdom or Baseline Skill XP
            if (AttunementManager.TryGetActivePower(player, TrophyId.Dvergr, out float dvergrXpBonus))
            {
                rested.m_raiseSkill = Skills.SkillType.All;
                rested.m_raiseSkillModifier = dvergrXpBonus * 0.01f;
            }
            else if (ModConfig.EnableBaselineRestedBuff.Value)
            {
                rested.m_raiseSkill = Skills.SkillType.All;
                rested.m_raiseSkillModifier = ModConfig.BaselineSkillXP.Value - 1f;
            }
            else
            {
                rested.m_raiseSkill = Skills.SkillType.None;
                rested.m_raiseSkillModifier = 0.0f;
            }

            // 3. Abomination: Rested duration scaling (1 min per comfort level instead of 20s)
            bool hasAbom = HasAbominationRite(player, playerComfort);
            if (!AbominationActiveMap.TryGetValue(rested, out bool wasAbomActive))
            {
                AbominationActiveMap[rested] = hasAbom;
            }
            else if (wasAbomActive != hasAbom)
            {
                float oldExpectedTtl = CalculateRestedDuration(playerComfort, wasAbomActive);
                float newExpectedTtl = CalculateRestedDuration(playerComfort, hasAbom);
                float ttlDiff = newExpectedTtl - oldExpectedTtl;
                rested.m_ttl += ttlDiff;
                if (StatusEffectTimeRef(rested) > rested.m_ttl)
                {
                    StatusEffectTimeRef(rested) = rested.m_ttl;
                }
                AbominationActiveMap[rested] = hasAbom;
            }

            // 4. Update HUD display name with ACTIVE rites only
            var names = new List<string>();
            foreach (var entry in activeEntries)
            {
                if (RiteDatabase.TryGetRite(entry.PrefabName, out TrophyRite rite))
                {
                    names.Add(rite.CreatureName);
                }
            }

            int comfort = GetComfortLevel(rested);
            string baseName = Localization.instance != null ? Localization.instance.Localize("$se_rested_name") : "Rested";
            if (names.Count > 0)
            {
                rested.m_name = $"{baseName} {comfort} ({string.Join(", ", names)})";
            }
            else
            {
                rested.m_name = $"{baseName} {comfort}";
            }
        }

        public static string FormatRestedTooltip(SE_Rested rested)
        {
            var sb = new System.Text.StringBuilder();

            // 1. Vanilla baseline stats (only if baseline buff is enabled)
            if (ModConfig.EnableBaselineRestedBuff.Value)
            {
                float hpBonus = (ModConfig.BaselineHealthRegen.Value - 1f) * 100f;
                float staBonus = (ModConfig.BaselineStaminaRegen.Value - 1f) * 100f;
                float eitrBonus = (ModConfig.BaselineEitrRegen.Value - 1f) * 100f;
                float xpBonus = (ModConfig.BaselineSkillXP.Value - 1f) * 100f;

                string hpLabel = Localization.instance != null ? Localization.instance.Localize("$se_healthregen") : "Health regen";
                string staLabel = Localization.instance != null ? Localization.instance.Localize("$se_staminaregen") : "Stamina regen";
                string eitrLabel = Localization.instance != null ? Localization.instance.Localize("$se_eitrregen") : "Eitr regen";
                string xpLabel = Localization.instance != null ? Localization.instance.Localize("$se_skillgain") : "Skill improvement";

                if (Math.Abs(hpBonus) > 0.01f) sb.Append($"{hpLabel}: +{hpBonus:0}%\n");
                if (Math.Abs(staBonus) > 0.01f) sb.Append($"{staLabel}: +{staBonus:0}%\n");
                if (Math.Abs(eitrBonus) > 0.01f) sb.Append($"{eitrLabel}: +{eitrBonus:0}%\n");
                if (Math.Abs(xpBonus) > 0.01f) sb.Append($"{xpLabel}: +{xpBonus:0}%\n");
            }

            // 2. Duration (Remaining and Total)
            float rem = Mathf.Max(0f, rested.GetRemaningTime());
            int remMin = Mathf.FloorToInt(rem / 60f);
            int remSec = Mathf.FloorToInt(rem % 60f);
            string remStr = remMin > 0 ? $"{remMin}m {remSec:D2}s" : $"{remSec}s";

            float total = rested.m_ttl;
            int totalMin = Mathf.FloorToInt(total / 60f);
            int totalSec = Mathf.FloorToInt(total % 60f);
            string totalStr = totalMin > 0 && totalSec == 0 ? $"{totalMin}m" : (totalMin > 0 ? $"{totalMin}m {totalSec:D2}s" : $"{totalSec}s");

            string remLabel = Localization.instance != null ? Localization.instance.Localize("$se_ttl") : "Remaining";
            sb.Append($"{remLabel}: {remStr} (Total: {totalStr})");

            return sb.ToString().TrimEnd();
        }
    }

    [HarmonyPatch(typeof(SE_Stats), nameof(SE_Stats.GetTooltipString))]
    public static class SEStatsTooltipPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(SE_Stats __instance, ref string __result)
        {
            if (!ModConfig.ModEnabled.Value || __instance is not SE_Rested rested)
            {
                return true;
            }

            __result = SERestedPatch.FormatRestedTooltip(rested);
            return false;
        }
    }

    [HarmonyPatch(typeof(StatusEffect), nameof(StatusEffect.GetTooltipString))]
    public static class StatusEffectTooltipPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(StatusEffect __instance, ref string __result)
        {
            if (!ModConfig.ModEnabled.Value || __instance is not SE_Rested rested)
            {
                return true;
            }

            __result = SERestedPatch.FormatRestedTooltip(rested);
            return false;
        }
    }

    [HarmonyPatch(typeof(StatusEffect), nameof(StatusEffect.Stop))]
    public static class StatusEffectStopPatch
    {
        [HarmonyPostfix]
        public static void Postfix(StatusEffect __instance)
        {
            if (__instance is SE_Rested rested)
            {
                SERestedPatch.OnRestedStop(rested);
            }
        }
    }

    [HarmonyPatch(typeof(SEMan))]
    public static class SEManPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SEMan.AddStatusEffect), typeof(StatusEffect), typeof(bool), typeof(int), typeof(float), typeof(short))]
        public static void AddStatusEffect_StatusEffect_Postfix(StatusEffect __result, Character ___m_character)
        {
            if (__result is SE_Rested rested && (___m_character as Player ?? Player.m_localPlayer) is Player player)
            {
                SERestedPatch.ApplyCustomModifiers(rested, player);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SEMan.AddStatusEffect), typeof(int), typeof(bool), typeof(int), typeof(float), typeof(short))]
        public static void AddStatusEffect_Int_Postfix(StatusEffect __result, Character ___m_character)
        {
            if (__result is SE_Rested rested && (___m_character as Player ?? Player.m_localPlayer) is Player player)
            {
                SERestedPatch.ApplyCustomModifiers(rested, player);
            }
        }
    }
}
