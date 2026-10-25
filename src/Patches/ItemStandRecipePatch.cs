using HarmonyLib;
using RestedRites.Configuration;
using UnityEngine;

namespace RestedRites.Patches
{
    [HarmonyPatch(typeof(ZNetScene), "Awake")]
    public static class ItemStandRecipePatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!ModConfig.ModEnabled.Value || !ModConfig.EarlyGameItemStands.Value || ZNetScene.instance == null)
            {
                return;
            }

            var woodItem = ZNetScene.instance.GetPrefab("Wood")?.GetComponent<ItemDrop>();
            var leatherItem = ZNetScene.instance.GetPrefab("LeatherScraps")?.GetComponent<ItemDrop>();

            if (woodItem == null || leatherItem == null)
            {
                RestedRitesPlugin.Log.LogWarning("[ItemStandRecipePatch] Could not find Wood or LeatherScraps prefabs to update item stand recipes.");
                return;
            }

            UpdateStandRecipe("itemstand", woodItem, leatherItem);
            UpdateStandRecipe("itemstandh", woodItem, leatherItem);
        }

        private static void UpdateStandRecipe(string prefabName, ItemDrop wood, ItemDrop leather)
        {
            var standPrefab = ZNetScene.instance.GetPrefab(prefabName);
            if (standPrefab == null) return;

            var piece = standPrefab.GetComponent<Piece>();
            if (piece == null) return;

            piece.m_resources = new Piece.Requirement[]
            {
                new() { m_resItem = wood, m_amount = 4, m_recover = true },
                new() { m_resItem = leather, m_amount = 1, m_recover = true }
            };

            RestedRitesPlugin.Log.LogInfo($"[ItemStandRecipePatch] Updated {prefabName} recipe to 4 Wood + 1 Leather Scraps.");
        }
    }
}
