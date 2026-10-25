using BepInEx.Configuration;

namespace RestedRites.Configuration
{
    public enum ProgressionModel
    {
        Reservoir,
        Uniform,
        Custom
    }

    public static class ModConfig
    {
        public const string DefaultReservoirTable =
@"1: 100.0, 0.0, 0.0, 0.0
2: 100.0, 7.5, 0.0, 0.0
3: 100.0, 15.0, 0.0, 0.0
4: 100.0, 22.5, 0.0, 0.0
5: 100.0, 30.0, 0.0, 0.0
6: 100.0, 37.5, 0.0, 0.0
7: 100.0, 45.0, 0.0, 0.0
8: 100.0, 50.0, 2.5, 0.0
9: 100.0, 50.0, 10.0, 0.0
10: 100.0, 50.0, 17.5, 0.0
11: 100.0, 50.0, 25.0, 0.0
12: 100.0, 50.0, 32.5, 0.0
13: 100.0, 52.5, 37.5, 0.0
14: 100.0, 55.0, 42.5, 0.0
15: 100.0, 55.0, 42.5, 7.5
16: 100.0, 55.0, 42.5, 15.0
17: 100.0, 55.0, 42.5, 22.5
18: 100.0, 55.0, 42.5, 30.0
19: 100.0, 57.5, 45.0, 32.5
20: 100.0, 60.0, 47.5, 35.0
21: 100.0, 62.5, 50.0, 37.5
22: 100.0, 65.0, 52.5, 40.0
23: 100.0, 67.5, 55.0, 42.5
24: 100.0, 70.0, 57.5, 45.0";

        public const string DefaultUniformTable =
@"1: 100.0, 0.0, 0.0, 0.0
2: 53.8, 53.8, 0.0, 0.0
3: 57.5, 57.5, 0.0, 0.0
4: 61.3, 61.3, 0.0, 0.0
5: 65.0, 65.0, 0.0, 0.0
6: 68.8, 68.8, 0.0, 0.0
7: 72.5, 72.5, 0.0, 0.0
8: 50.8, 50.8, 50.8, 0.0
9: 53.3, 53.3, 53.3, 0.0
10: 55.8, 55.8, 55.8, 0.0
11: 58.3, 58.3, 58.3, 0.0
12: 60.8, 60.8, 60.8, 0.0
13: 63.3, 63.3, 63.3, 0.0
14: 65.8, 65.8, 65.8, 0.0
15: 51.3, 51.3, 51.3, 51.3
16: 53.1, 53.1, 53.1, 53.1
17: 55.0, 55.0, 55.0, 55.0
18: 56.9, 56.9, 56.9, 56.9
19: 58.8, 58.8, 58.8, 58.8
20: 60.6, 60.6, 60.6, 60.6
21: 62.5, 62.5, 62.5, 62.5
22: 64.4, 64.4, 64.4, 64.4
23: 66.3, 66.3, 66.3, 66.3
24: 68.1, 68.1, 68.1, 68.1";

        public static ConfigEntry<bool> ModEnabled { get; private set; } = null!;
        public static ConfigEntry<bool> EarlyGameItemStands { get; private set; } = null!;

        public static ConfigEntry<bool> EnableCustomDuration { get; private set; } = null!;
        public static ConfigEntry<float> BaseDurationMinutes { get; private set; } = null!;
        public static ConfigEntry<float> DurationPerComfortSeconds { get; private set; } = null!;

        public static ConfigEntry<ProgressionModel> ProgressionType { get; private set; } = null!;
        public static ConfigEntry<string> CustomProgressionTable { get; private set; } = null!;
        public static ConfigEntry<float> CommonScalingFactor { get; private set; } = null!;

        public static ConfigEntry<int> Checkpoint1 { get; private set; } = null!;
        public static ConfigEntry<int> Checkpoint2 { get; private set; } = null!;
        public static ConfigEntry<int> Checkpoint3 { get; private set; } = null!;
        public static ConfigEntry<int> Checkpoint4 { get; private set; } = null!;
        public static ConfigEntry<float> PowerIncrement { get; private set; } = null!;

        public static ConfigEntry<bool> EnableBaselineRestedBuff { get; private set; } = null!;
        public static ConfigEntry<float> BaselineHealthRegen { get; private set; } = null!;
        public static ConfigEntry<float> BaselineStaminaRegen { get; private set; } = null!;
        public static ConfigEntry<float> BaselineEitrRegen { get; private set; } = null!;
        public static ConfigEntry<float> BaselineSkillXP { get; private set; } = null!;

        public static ConfigEntry<bool> DisplayModifiedTooltips { get; private set; } = null!;
        public static ConfigEntry<bool> EnableSensoryFeedback { get; private set; } = null!;
        public static ConfigEntry<bool> EnableCombatFloatingText { get; private set; } = null!;
        public static ConfigEntry<bool> DebugCombatLogging { get; private set; } = null!;

        public static void Init(ConfigFile config)
        {
            ModEnabled = Bind(
                config,
                "1 - General",
                "Enabled",
                true,
                "Master switch to enable or disable all features of RestedRites.",
                order: 100,
                isAdminOnly: true
            );

            EarlyGameItemStands = Bind(
                config,
                "1 - General",
                "EarlyGameItemStands",
                true,
                "Lowers the recipe of item stands (both vertical and horizontal) from Fine Wood and Bronze Nails to 4 Wood and 1 Leather Scraps, enabling trophy mounting and rite attunement in the Meadows.",
                order: 90,
                isAdminOnly: true
            );

            EnableCustomDuration = Bind(
                config,
                "2 - Rested Duration",
                "EnableCustomDuration",
                true,
                "Master toggle ('vanilla killswitch') for custom Rested duration. When disabled (false), duration reverts to vanilla formula (8 min base + 1 min per comfort level above 1). If Abomination is active, it extends upon vanilla duration.",
                order: 100,
                isAdminOnly: true
            );

            BaseDurationMinutes = Bind(
                config,
                "2 - Rested Duration",
                "BaseDurationMinutes",
                10.0f,
                "Base Rested duration in minutes at Comfort 1 under the custom linear formula (default: 10.0 minutes).",
                order: 90,
                isAdminOnly: true
            );

            DurationPerComfortSeconds = Bind(
                config,
                "2 - Rested Duration",
                "DurationPerComfortSeconds",
                20.0f,
                "Extra Rested duration added per comfort level above Comfort 1 (default: 20.0 seconds).",
                order: 80,
                isAdminOnly: true
            );

            ProgressionType = Bind(
                config,
                "3 - Power & Progression",
                "ProgressionType",
                ProgressionModel.Reservoir,
                "Selects the power progression model across Comfort 1-24.\nReservoir: Primary anchor starts at 100%, cascading surplus into supporting slots (never weakens the main trophy).\nUniform: Equal distribution across active slots, redistributing at Comfort 2, 8, 15.\nCustom: Uses the 24x4 CustomProgressionTable below.",
                order: 110,
                isAdminOnly: true
            );

            CustomProgressionTable = Bind(
                config,
                "3 - Power & Progression",
                "CustomProgressionTable",
                DefaultReservoirTable,
                "24 rows representing Comfort 1 to 24. Each row defines the 4 slot percentages (0.0 to 100.0) separated by commas.\nFormat: 'Level: Slot1, Slot2, Slot3, Slot4' or 'Slot1, Slot2, Slot3, Slot4'. Active when ProgressionType is set to Custom.",
                order: 105,
                isAdminOnly: true
            );

            Checkpoint1 = Bind(
                config,
                "3 - Power & Progression",
                "Checkpoint1",
                1,
                "[Legacy] First comfort checkpoint to unlock an attunement slot.",
                order: 100,
                isAdminOnly: true
            );

            Checkpoint2 = Bind(
                config,
                "3 - Power & Progression",
                "Checkpoint2",
                5,
                "Second comfort checkpoint to unlock an attunement slot (order does not matter).",
                order: 90,
                isAdminOnly: true
            );

            Checkpoint3 = Bind(
                config,
                "3 - Power & Progression",
                "Checkpoint3",
                10,
                "Third comfort checkpoint to unlock an attunement slot (order does not matter).",
                order: 80,
                isAdminOnly: true
            );

            Checkpoint4 = Bind(
                config,
                "3 - Power & Progression",
                "Checkpoint4",
                15,
                "Fourth comfort checkpoint to unlock an attunement slot (order does not matter).",
                order: 70,
                isAdminOnly: true
            );

            PowerIncrement = Bind(
                config,
                "3 - Power & Progression",
                "PowerIncrement",
                0.075f,
                "Power increment (pwr_increment) added to the power budget per comfort level above 1 (default: 0.075 = 7.5% per level).",
                order: 60,
                isAdminOnly: true
            );

            CommonScalingFactor = Bind(
                config,
                "3 - Power & Progression",
                "CommonScalingFactor",
                1.0f,
                "Global scaling factor that multiplies the potency of all attuned trophy rites (default: 1.0).",
                order: 50,
                isAdminOnly: true
            );

            EnableBaselineRestedBuff = Bind(
                config,
                "4 - Baseline Rested",
                "EnableBaselineRestedBuff",
                false,
                "Enables baseline vanilla Rested buff regeneration and skill multipliers. When disabled (default: false), baseline Rested multipliers are power-neutral (1.0x regen, 0% bonus XP), leaving all stat boosts to attuned trophy rites.",
                order: 100,
                isAdminOnly: true
            );

            BaselineHealthRegen = Bind(
                config,
                "4 - Baseline Rested",
                "BaselineHealthRegen",
                1.5f,
                "Baseline health regen multiplier when Rested (Vanilla default is 1.5 = +50% health regen).",
                order: 90,
                isAdminOnly: true
            );

            BaselineStaminaRegen = Bind(
                config,
                "4 - Baseline Rested",
                "BaselineStaminaRegen",
                2.0f,
                "Baseline stamina regen multiplier when Rested (Vanilla default is 2.0 = +100% stamina regen).",
                order: 80,
                isAdminOnly: true
            );

            BaselineEitrRegen = Bind(
                config,
                "4 - Baseline Rested",
                "BaselineEitrRegen",
                2.0f,
                "Baseline eitr regen multiplier when Rested (Vanilla default is 2.0 = +100% eitr regen).",
                order: 70,
                isAdminOnly: true
            );

            BaselineSkillXP = Bind(
                config,
                "4 - Baseline Rested",
                "BaselineSkillXP",
                1.5f,
                "Baseline skill XP multiplier when Rested (Vanilla default is 1.5 = +50% increased skill XP).",
                order: 60,
                isAdminOnly: true
            );

            DisplayModifiedTooltips = Bind(
                config,
                "5 - UI & Feedback",
                "DisplayModifiedTooltips",
                true,
                "Displays dynamically buffed values for damage, stamina, knockback, backstab, and stagger directly on weapon tooltips.",
                order: 100,
                isAdminOnly: false
            );

            EnableSensoryFeedback = Bind(
                config,
                "5 - UI & Feedback",
                "EnableSensoryFeedback",
                true,
                "Plays subtle native visual particle effects and audio cues on trophy attunement, defensive wards, and major combat procs.",
                order: 90,
                isAdminOnly: false
            );

            EnableCombatFloatingText = Bind(
                config,
                "5 - UI & Feedback",
                "EnableCombatFloatingText",
                true,
                "Shows native floating combat text for Carapace Ward deflection and healing procs.",
                order: 80,
                isAdminOnly: false
            );

            DebugCombatLogging = Bind(
                config,
                "6 - Debug",
                "DebugCombatLogging",
                false,
                "Logs detailed combat calculations (base roll, rite multiplier, target armor reduction, and final damage) to the BepInEx console and HUD.",
                order: 10,
                isAdminOnly: false
            );
        }

        private static ConfigEntry<T> Bind<T>(
            ConfigFile config,
            string section,
            string key,
            T defaultValue,
            string description,
            int order = 0,
            AcceptableValueBase? acceptableValues = null,
            bool isAdminOnly = false)
        {
            var tags = new ConfigurationManagerAttributes
            {
                Order = order,
                Category = section,
                IsAdminOnly = isAdminOnly
            };
            var configDesc = new ConfigDescription(description, acceptableValues, tags);
            return config.Bind(section, key, defaultValue, configDesc);
        }
    }
}
