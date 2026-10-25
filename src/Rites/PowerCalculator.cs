using System;
using System.Globalization;
using RestedRites.Configuration;
using UnityEngine;

namespace RestedRites.Rites
{
    public static class PowerCalculator
    {
        public const int MaxComfortLevels = 24;
        public const int MaxSlots = 4;

        // Rows 1..24, Columns 0..3 (percent values 0.0 to 100.0)
        private static readonly float[,] s_table = new float[MaxComfortLevels + 1, MaxSlots];
        private static bool s_initialized = false;

        public static void Initialize()
        {
            ReloadTable();
        }

        public static void ReloadTable()
        {
            string rawTable = ModConfig.ProgressionType.Value switch
            {
                ProgressionModel.Reservoir => ModConfig.DefaultReservoirTable,
                ProgressionModel.Uniform => ModConfig.DefaultUniformTable,
                ProgressionModel.Custom => string.IsNullOrWhiteSpace(ModConfig.CustomProgressionTable.Value)
                    ? ModConfig.DefaultReservoirTable
                    : ModConfig.CustomProgressionTable.Value,
                _ => ModConfig.DefaultReservoirTable
            };

            ParseTable(rawTable);
            s_initialized = true;
        }

        private static void ParseTable(string raw)
        {
            Array.Clear(s_table, 0, s_table.Length);

            string[] lines = raw.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int comfortIndex = 1;

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#") || trimmed.StartsWith("//"))
                {
                    continue;
                }

                int colonIdx = trimmed.IndexOf(':');
                string valuesPart = colonIdx >= 0 ? trimmed.Substring(colonIdx + 1) : trimmed;

                string[] parts = valuesPart.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int slot = 0; slot < MaxSlots && slot < parts.Length; slot++)
                {
                    if (float.TryParse(parts[slot], NumberStyles.Float, CultureInfo.InvariantCulture, out float pct))
                    {
                        s_table[comfortIndex, slot] = Mathf.Clamp(pct, 0f, 100f);
                    }
                }

                comfortIndex++;
                if (comfortIndex > MaxComfortLevels) break;
            }
        }

        public static int ClampComfort(int comfort)
        {
            if (comfort < 1) return 1;
            if (comfort > MaxComfortLevels) return MaxComfortLevels;
            return comfort;
        }

        public static int GetActiveSlots(int comfort)
        {
            if (!s_initialized) ReloadTable();
            int c = ClampComfort(comfort);

            int active = 0;
            for (int i = 0; i < MaxSlots; i++)
            {
                if (s_table[c, i] > 0.001f)
                {
                    active++;
                }
            }
            return active;
        }

        public static float GetSlotPercent(int slotIndex, int comfort)
        {
            if (!s_initialized) ReloadTable();
            if (slotIndex < 0 || slotIndex >= MaxSlots) return 0f;
            int c = ClampComfort(comfort);
            return s_table[c, slotIndex];
        }

        public static float GetSlotMultiplier(int slotIndex, int comfort)
        {
            float pct = GetSlotPercent(slotIndex, comfort);
            return (pct * 0.01f) * ModConfig.CommonScalingFactor.Value;
        }

        public static float CalculatePower(float basePower, int comfort, int slotIndex = 0)
        {
            return basePower * GetSlotMultiplier(slotIndex, comfort);
        }

        public static float CalculatePowerMultiplier(int comfort, int slotIndex = 0)
        {
            return GetSlotMultiplier(slotIndex, comfort);
        }
    }
}
