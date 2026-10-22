using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestedRites.Configuration;
using RestedRites.Patches;
using RestedRites.Rites;

namespace RestedRites.Services
{
    public class AttunedEntry
    {
        public string PrefabName { get; }

        public AttunedEntry(string prefabName)
        {
            PrefabName = prefabName;
        }

        public override string ToString() => PrefabName;

        public static AttunedEntry? Parse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            string[] parts = raw.Split(':');
            string prefab = parts[0].Trim();
            return string.IsNullOrEmpty(prefab) ? null : new AttunedEntry(prefab);
        }
    }

    public static class AttunementManager
    {
        public const string CustomDataKey = "RestedRites_AttunedTrophies";
        public const int MaxStoredTrophies = 4;

        public static int GetMaxSlots(int comfortLevel)
        {
            return PowerCalculator.GetActiveSlots(comfortLevel);
        }

        public static int GetRequiredComfortForSlot(int slotIndex)
        {
            int[] checkpoints = {
                ModConfig.Checkpoint1.Value,
                ModConfig.Checkpoint2.Value,
                ModConfig.Checkpoint3.Value,
                ModConfig.Checkpoint4.Value
            };
            Array.Sort(checkpoints);

            if (slotIndex < 0) return checkpoints[0];
            if (slotIndex < checkpoints.Length) return checkpoints[slotIndex];
            return checkpoints[checkpoints.Length - 1];
        }

        public static List<AttunedEntry> GetAttunedEntries(Player player)
        {
            var list = new List<AttunedEntry>();
            if (player == null || player.m_customData == null) return list;

            if (player.m_customData.TryGetValue(CustomDataKey, out string raw) && !string.IsNullOrWhiteSpace(raw))
            {
                string[] split = raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in split)
                {
                    var entry = AttunedEntry.Parse(item);
                    if (entry != null && !list.Any(e => string.Equals(e.PrefabName, entry.PrefabName, StringComparison.OrdinalIgnoreCase)))
                    {
                        list.Add(entry);
                        if (list.Count >= MaxStoredTrophies) break;
                    }
                }
            }

            return list;
        }

        public static List<AttunedEntry> GetActiveEntries(Player player, int playerComfort)
        {
            int activeSlots = GetMaxSlots(playerComfort);
            return GetAttunedEntries(player).Take(activeSlots).ToList();
        }

        public static List<string> GetAttunedTrophies(Player player)
        {
            return GetAttunedEntries(player).Select(e => e.PrefabName).ToList();
        }

        public static void SetAttunedEntries(Player player, List<AttunedEntry> entries)
        {
            if (player == null || player.m_customData == null) return;

            if (entries == null || entries.Count == 0)
            {
                player.m_customData.Remove(CustomDataKey);
            }
            else
            {
                var capped = entries.Take(MaxStoredTrophies).ToList();
                player.m_customData[CustomDataKey] = string.Join(",", capped.Select(e => e.ToString()));
            }
        }

        public static bool IsAttuned(Player player, string trophyPrefab)
        {
            if (string.IsNullOrEmpty(trophyPrefab)) return false;
            return GetAttunedEntries(player).Any(e => string.Equals(e.PrefabName, trophyPrefab, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsActive(Player player, string trophyPrefab, int playerComfort)
        {
            if (string.IsNullOrEmpty(trophyPrefab)) return false;
            return GetActiveEntries(player, playerComfort).Any(e => string.Equals(e.PrefabName, trophyPrefab, StringComparison.OrdinalIgnoreCase));
        }

        public static bool TryGetActivePower(Player? player, TrophyId id, out float power)
        {
            power = 0f;
            if (player == null) return false;

            // Only active when player has the Rested status effect
            var rested = player.GetSEMan()?.GetStatusEffect(StringExtensionMethods.GetStableHashCode("Rested")) as SE_Rested;
            if (rested == null) return false;

            int playerComfort = SERestedPatch.GetComfortLevel(rested);
            int maxSlots = PowerCalculator.GetActiveSlots(playerComfort);
            if (maxSlots <= 0) return false;

            var activeEntries = GetActiveEntries(player, playerComfort);
            if (activeEntries.Count == 0) return false;

            foreach (var entry in activeEntries)
            {
                if (RiteDatabase.TryGetRite(entry.PrefabName, out TrophyRite rite) && rite.Id == id)
                {
                    power = PowerCalculator.CalculatePower(rite.BaseMagnitude, playerComfort);
                    return true;
                }
            }

            return false;
        }

        public static Dictionary<SetId, int> GetActiveSetCounts(Player? player)
        {
            var counts = new Dictionary<SetId, int>();
            if (player == null) return counts;

            var rested = player.GetSEMan()?.GetStatusEffect(StringExtensionMethods.GetStableHashCode("Rested")) as SE_Rested;
            if (rested == null) return counts;

            int playerComfort = SERestedPatch.GetComfortLevel(rested);
            var activeEntries = GetActiveEntries(player, playerComfort);

            foreach (var entry in activeEntries)
            {
                if (RiteDatabase.TryGetRite(entry.PrefabName, out TrophyRite rite) &&
                    SetDatabase.TryGetSetForTrophy(rite.Id, out TrophySet set))
                {
                    counts[set.Id] = counts.TryGetValue(set.Id, out int c) ? c + 1 : 1;
                }
            }

            return counts;
        }

        public static bool HasActiveSetBonus(Player? player, SetId setId, int piecesRequired)
        {
            if (player == null) return false;

            var rested = player.GetSEMan()?.GetStatusEffect(StringExtensionMethods.GetStableHashCode("Rested")) as SE_Rested;
            if (rested == null) return false;

            int playerComfort = SERestedPatch.GetComfortLevel(rested);
            var activeEntries = GetActiveEntries(player, playerComfort);

            int count = 0;
            foreach (var entry in activeEntries)
            {
                if (RiteDatabase.TryGetRite(entry.PrefabName, out TrophyRite rite) &&
                    SetDatabase.TryGetSetForTrophy(rite.Id, out TrophySet set) &&
                    set.Id == setId)
                {
                    count++;
                    if (count >= piecesRequired) return true;
                }
            }

            return false;
        }

        public static bool AttuneTrophy(Player player, string trophyPrefab, out string message)
        {
            if (player == null)
            {
                message = "Player not found.";
                return false;
            }

            if (!RiteDatabase.TryGetRite(trophyPrefab, out TrophyRite rite))
            {
                message = $"'{trophyPrefab}' is not a recognized trophy rite.";
                return false;
            }

            var entries = GetAttunedEntries(player);

            // Check if creature already in list
            int existingIndex = entries.FindIndex(e => string.Equals(e.PrefabName, trophyPrefab, StringComparison.OrdinalIgnoreCase));
            if (existingIndex >= 0)
            {
                var existing = entries[existingIndex];
                // Bring to index 0 (Most Recently Used / Slot 1) so it is guaranteed active
                entries.RemoveAt(existingIndex);
                entries.Insert(0, existing);
                SetAttunedEntries(player, entries);

                SERestedPatch.ReapplyCustomModifiers(player);
                message = $"Attuned: {rite.DisplayName} ({rite.CreatureName})";
                return true;
            }

            // Creature is not in list
            entries.Insert(0, new AttunedEntry(trophyPrefab));

            if (entries.Count > MaxStoredTrophies)
            {
                entries.RemoveRange(MaxStoredTrophies, entries.Count - MaxStoredTrophies);
            }

            SetAttunedEntries(player, entries);

            SERestedPatch.ReapplyCustomModifiers(player);
            message = $"Attuned: {rite.DisplayName} ({rite.CreatureName})";
            return true;
        }

        public static string FormatTooltip(Player player, int playerComfort)
        {
            var entries = GetAttunedEntries(player);
            int activeSlots = GetMaxSlots(playerComfort);
            int activeCount = Math.Min(entries.Count, activeSlots);
            float pwrIncrement = ModConfig.PowerIncrement.Value;
            float totalPower = 1f + (playerComfort - 1f) * pwrIncrement;
            float scalingPercent = activeSlots > 0 ? (totalPower / activeSlots) * 100f * ModConfig.CommonScalingFactor.Value : 0f;

            var sb = new StringBuilder();
            sb.Append($"\n\n<color=#FFD700><b>Attuned Rites ({activeCount}/{activeSlots} Active • {scalingPercent:0.#}% Power Scaling, {entries.Count}/{MaxStoredTrophies} Stored):</b></color>");

            if (entries.Count == 0)
            {
                sb.Append("\n<color=#A0A0A0>• None active. Mount trophies on Item Stands and press Shift+E to attune.</color>");
                return sb.ToString();
            }

            // Active entries
            for (int i = 0; i < activeCount; i++)
            {
                var entry = entries[i];
                if (RiteDatabase.TryGetRite(entry.PrefabName, out TrophyRite rite))
                {
                    string bonus = rite.GetFormattedDescription(playerComfort);
                    sb.Append($"\n• <color=#00FFFF>{rite.DisplayName}</color> ({rite.CreatureName}): <color=#80FF80>{bonus}</color>");
                }
                else
                {
                    sb.Append($"\n• <color=#00FFFF>{entry.PrefabName}</color>: <color=#80FF80>Active</color>");
                }
            }

            // Empty active slots
            for (int i = activeCount; i < activeSlots; i++)
            {
                sb.Append($"\n<color=#80FF80>• Slot {i + 1}: Empty (Mount trophy & Shift+E to attune)</color>");
            }

            // Inactive (stored) entries
            for (int i = activeCount; i < entries.Count; i++)
            {
                var entry = entries[i];
                int neededComfort = GetRequiredComfortForSlot(i);
                if (RiteDatabase.TryGetRite(entry.PrefabName, out TrophyRite rite))
                {
                    sb.Append($"\n<color=#A0A0A0>• {rite.DisplayName} ({rite.CreatureName}): Stored (Requires Comfort {neededComfort}+)</color>");
                }
                else
                {
                    sb.Append($"\n<color=#A0A0A0>• {entry.PrefabName}: Stored (Requires Comfort {neededComfort}+)</color>");
                }
            }

            // Set Synergies
            var activeSetCounts = new Dictionary<SetId, int>();
            for (int i = 0; i < activeCount; i++)
            {
                var entry = entries[i];
                if (RiteDatabase.TryGetRite(entry.PrefabName, out TrophyRite rite) &&
                    SetDatabase.TryGetSetForTrophy(rite.Id, out TrophySet set))
                {
                    activeSetCounts[set.Id] = activeSetCounts.TryGetValue(set.Id, out int c) ? c + 1 : 1;
                }
            }

            if (activeSetCounts.Count > 0)
            {
                sb.Append("\n\n<color=#FFD700><b>Set Synergies:</b></color>");
                foreach (var kvp in activeSetCounts)
                {
                    if (SetDatabase.TryGetSet(kvp.Key, out TrophySet set))
                    {
                        int pieces = kvp.Value;
                        int total = set.Members.Length;
                        sb.Append($"\n• <color=#00FFFF>{set.Name}</color> ({pieces}/{total} Pieces):");

                        if (set.TwoPieceBonus != null)
                        {
                            string state = pieces >= 2 ? "<color=#80FF80>[ACTIVE]</color>" : "<color=#A0A0A0>[Requires 2]</color>";
                            string color = pieces >= 2 ? "#80FF80" : "#A0A0A0";
                            sb.Append($"\n   <color={color}>(2) {set.TwoPieceBonus} {state}</color>");
                        }
                        if (set.ThreePieceBonus != null)
                        {
                            string state = pieces >= 3 ? "<color=#80FF80>[ACTIVE]</color>" : "<color=#A0A0A0>[Requires 3]</color>";
                            string color = pieces >= 3 ? "#80FF80" : "#A0A0A0";
                            sb.Append($"\n   <color={color}>(3) {set.ThreePieceBonus} {state}</color>");
                        }
                        if (set.FourPieceBonus != null)
                        {
                            string state = pieces >= 4 ? "<color=#80FF80>[ACTIVE]</color>" : "<color=#A0A0A0>[Requires 4]</color>";
                            string color = pieces >= 4 ? "#80FF80" : "#A0A0A0";
                            sb.Append($"\n   <color={color}>(4) {set.FourPieceBonus} {state}</color>");
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}
