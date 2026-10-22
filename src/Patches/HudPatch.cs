using System.Collections.Generic;
using HarmonyLib;
using RestedRites.Configuration;
using RestedRites.Services;
using UnityEngine;
using UnityEngine.UI;

namespace RestedRites.Patches
{
    [HarmonyPatch(typeof(Hud))]
    public static class HudPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UpdateStatusEffects")]
        public static void UpdateStatusEffects_Postfix(Hud __instance, List<StatusEffect> statusEffects, List<RectTransform> ___m_statusEffects)
        {
            if (!ModConfig.ModEnabled.Value || statusEffects == null || ___m_statusEffects == null)
            {
                return;
            }

            var player = Player.m_localPlayer;
            if (player == null)
            {
                return;
            }

            for (int i = 0; i < statusEffects.Count; i++)
            {
                if (statusEffects[i] is SE_Rested rested && i < ___m_statusEffects.Count)
                {
                    RectTransform element = ___m_statusEffects[i];
                    if (element == null) continue;

                    // Ensure modifiers and m_name are applied
                    SERestedPatch.ApplyCustomModifiers(rested, player);

                    // Update HUD name text directly above icon
                    var nameText = element.Find("Name")?.GetComponent<TMPro.TMP_Text>();
                    if (nameText != null && !string.IsNullOrEmpty(rested.m_name))
                    {
                        nameText.text = Localization.instance != null ? Localization.instance.Localize(rested.m_name) : rested.m_name;
                    }

                    // Ensure icon image has raycastTarget enabled for hover tooltips
                    var iconImage = element.Find("Icon")?.GetComponent<Image>();
                    if (iconImage != null)
                    {
                        iconImage.raycastTarget = true;
                    }

                    // Attach or update UITooltip on the element
                    var tooltip = element.GetComponent<UITooltip>();
                    if (tooltip == null)
                    {
                        tooltip = element.gameObject.AddComponent<UITooltip>();
                    }

                    int comfort = SERestedPatch.GetComfortLevel(rested);
                    tooltip.m_topic = Localization.instance != null ? Localization.instance.Localize(rested.m_name) : rested.m_name;
                    tooltip.m_text = SERestedPatch.FormatRestedTooltip(rested);

                    break;
                }
            }
        }
    }
}
