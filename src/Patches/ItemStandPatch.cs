using System;
using System.Text;
using HarmonyLib;
using RestedRites.Configuration;
using RestedRites.Rites;
using RestedRites.Services;
using UnityEngine;

namespace RestedRites.Patches
{
    [HarmonyPatch(typeof(ItemStand))]
    public static class ItemStandPatch
    {
        public static GameObject? GetAttachedItemPrefabObject(ItemStand stand)
        {
            if (stand == null) return null;

            int itemHash = stand.GetAttachedItem();
            if (itemHash != 0 && ObjectDB.instance != null)
            {
                return ObjectDB.instance.GetItemPrefab(itemHash);
            }

            return null;
        }

        public static string GetAttachedItemPrefab(ItemStand stand)
        {
            var prefab = GetAttachedItemPrefabObject(stand);
            return prefab != null ? CleanPrefabName(prefab.name) : string.Empty;
        }

        private static string CleanPrefabName(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return string.Empty;
            return raw.Replace("(Clone)", "").Trim();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ItemStand.GetHoverText))]
        public static void GetHoverText_Postfix(ItemStand __instance, ref string __result)
        {
            if (!ModConfig.ModEnabled.Value || __instance == null || !__instance.HaveAttachment())
            {
                return;
            }

            var prefabObj = GetAttachedItemPrefabObject(__instance);
            if (prefabObj == null) return;

            string itemPrefab = CleanPrefabName(prefabObj.name);
            if (string.IsNullOrEmpty(itemPrefab) || !RiteDatabase.TryGetRite(itemPrefab, out TrophyRite rite))
            {
                return;
            }

            var player = Player.m_localPlayer;
            if (player == null)
            {
                return;
            }

            var sb = new StringBuilder();

            // Line 1: Item stand (CREATURE trophy)
            string standName = Localization.instance != null ? Localization.instance.Localize(__instance.m_name) : "Item stand";
            var itemDrop = prefabObj.GetComponent<ItemDrop>();
            string trophyName = (itemDrop?.m_itemData?.m_shared?.m_name != null && Localization.instance != null)
                ? Localization.instance.Localize(itemDrop.m_itemData.m_shared.m_name)
                : rite.CreatureName + " trophy";
            sb.Append($"{standName} ({trophyName})");

            // Line 2: [Hold E] Take item
            sb.Append("\n[<color=yellow><b>$ui_hold $KEY_Use</b></color>] $piece_itemstand_take");

            // Line 3: [Shift + E] Attune(d): BUFF_NAME
            bool isAttuned = AttunementManager.IsAttuned(player, rite.PrefabName);
            string action = isAttuned ? "Attuned" : "Attune";
            sb.Append($"\n[<color=yellow><b>Shift + E</b></color>] {action}: <color=#00FFFF>{rite.DisplayName}</color>");

            // Line 4: Rite: BUFF_DESCRIPTION (base unscaled power)
            sb.Append($"\nRite: <color=#80FF80>{rite.GetBaseDescription()}</color>");

            // Line 5: Set: SET_NAME (attuned/available) - hidden if wildcard
            if (SetDatabase.TryGetSetForTrophy(rite.Id, out TrophySet set))
            {
                var activeCounts = AttunementManager.GetActiveSetCounts(player);
                activeCounts.TryGetValue(set.Id, out int pieces);
                sb.Append($"\nSet: <color=#00FFFF>{set.Name}</color> ({pieces}/{set.Members.Length} Active)");
            }

            __result = Localization.instance != null ? Localization.instance.Localize(sb.ToString()) : sb.ToString();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ItemStand.Interact))]
        public static bool Interact_Prefix(ItemStand __instance, Humanoid user, bool hold, bool alt)
        {
            if (!ModConfig.ModEnabled.Value || __instance == null || hold)
            {
                return true; // Let normal interaction proceed
            }

            // Check if Shift is held
            bool isShiftHeld = alt || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (!isShiftHeld)
            {
                return true;
            }

            if (user is not Player player || !__instance.HaveAttachment())
            {
                return true;
            }

            string itemPrefab = GetAttachedItemPrefab(__instance);
            if (string.IsNullOrEmpty(itemPrefab) || !RiteDatabase.TryGetRite(itemPrefab, out TrophyRite rite))
            {
                return true;
            }

            AttunementManager.AttuneTrophy(player, rite.PrefabName, out string message);
            FeedbackManager.TriggerAttunement(__instance, rite.DisplayName);

            RestedRitesPlugin.Log.LogInfo($"[RestedRites] Player attuned rite '{rite.DisplayName}' ({rite.PrefabName}): {message}");

            // Block default take-item behavior
            return false;
        }
    }
}
