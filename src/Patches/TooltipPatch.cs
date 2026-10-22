using System;
using HarmonyLib;
using RestedRites.Configuration;
using RestedRites.Rites;
using RestedRites.Services;

namespace RestedRites.Patches
{
    [HarmonyPatch]
    public static class TooltipPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip), new Type[]
        {
            typeof(ItemDrop.ItemData),
            typeof(int),
            typeof(bool),
            typeof(float),
            typeof(int),
            typeof(bool)
        })]
        public static void GetTooltip_Postfix(
            ItemDrop.ItemData item,
            int qualityLevel,
            bool crafting,
            float worldLevel,
            int stackOverride,
            bool appending,
            ref string __result)
        {
            if (!ModConfig.ModEnabled.Value || !ModConfig.DisplayModifiedTooltips.Value || item?.m_shared == null)
            {
                return;
            }

            var player = Player.m_localPlayer;
            if (player == null)
            {
                return;
            }

            // RestedRites only handles Trophy tooltips.
            // Weapon and tool tooltips are comprehensively rendered by VikingLib's TooltipEngine.
            if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Trophy)
            {
                string prefabName = item.m_dropPrefab != null ? item.m_dropPrefab.name : item.m_shared.m_name;
                if (RiteDatabase.TryGetRite(prefabName, out TrophyRite rite))
                {
                    var sb = new System.Text.StringBuilder();
                    sb.Append($"\n\n<color=#FFD700><b>Rested Rite: {rite.DisplayName}</b></color>");
                    sb.Append($"\n<color=#80FF80>Base Power: {rite.GetBaseDescription()}</color>");

                    if (SetDatabase.TryGetSetForTrophy(rite.Id, out TrophySet set))
                    {
                        var activeCounts = AttunementManager.GetActiveSetCounts(player);
                        activeCounts.TryGetValue(set.Id, out int pieces);
                        int total = set.Members.Length;

                        sb.Append($"\n\n<color=#00FFFF><b>Set: {set.Name} ({pieces}/{total} Active)</b></color>");
                        if (set.TwoPieceBonus != null)
                        {
                            string state = pieces >= 2 ? "<color=#80FF80>[ACTIVE]</color>" : "<color=#A0A0A0>[Requires 2]</color>";
                            string color = pieces >= 2 ? "#80FF80" : "#A0A0A0";
                            sb.Append($"\n<color={color}>• (2) {set.TwoPieceBonus} {state}</color>");
                        }
                        if (set.ThreePieceBonus != null)
                        {
                            string state = pieces >= 3 ? "<color=#80FF80>[ACTIVE]</color>" : "<color=#A0A0A0>[Requires 3]</color>";
                            string color = pieces >= 3 ? "#80FF80" : "#A0A0A0";
                            sb.Append($"\n<color={color}>• (3) {set.ThreePieceBonus} {state}</color>");
                        }
                        if (set.FourPieceBonus != null)
                        {
                            string state = pieces >= 4 ? "<color=#80FF80>[ACTIVE]</color>" : "<color=#A0A0A0>[Requires 4]</color>";
                            string color = pieces >= 4 ? "#80FF80" : "#A0A0A0";
                            sb.Append($"\n<color={color}>• (4) {set.FourPieceBonus} {state}</color>");
                        }
                    }
                    else
                    {
                        sb.Append("\n<color=#E0E0E0><i>Standalone Wildcard Rite</i></color>");
                    }

                    __result += sb.ToString();
                }
            }
        }
    }
}
