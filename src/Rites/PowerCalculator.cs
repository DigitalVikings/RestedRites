using System;
using RestedRites.Configuration;

namespace RestedRites.Rites
{
    public static class PowerCalculator
    {
        public static int GetActiveSlots(int comfort)
        {
            int count = 0;
            int[] checkpoints = {
                ModConfig.Checkpoint1.Value,
                ModConfig.Checkpoint2.Value,
                ModConfig.Checkpoint3.Value,
                ModConfig.Checkpoint4.Value
            };

            for (int i = 0; i < checkpoints.Length; i++)
            {
                if (comfort >= checkpoints[i])
                {
                    count++;
                }
            }

            return count; // 0 to 4
        }

        public static float CalculatePowerMultiplier(int comfort)
        {
            int n = GetActiveSlots(comfort);
            if (n <= 0)
            {
                return 0f;
            }

            float pwrIncrement = ModConfig.PowerIncrement.Value;
            float tablePwr = (1f + (comfort - 1f) * pwrIncrement) / n;
            return tablePwr * ModConfig.CommonScalingFactor.Value;
        }

        public static float CalculatePower(float basePower, int comfort)
        {
            return basePower * CalculatePowerMultiplier(comfort);
        }
    }
}
