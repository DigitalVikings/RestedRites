using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Utils;
using RestedRites.Configuration;
using RestedRites.Patches;

namespace RestedRites
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    [BepInDependency("com.digitalvikings.vikinglib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(Jotunn.Main.ModGuid, BepInDependency.DependencyFlags.HardDependency)]
    public class RestedRitesPlugin : BaseUnityPlugin
    {
        public const string ModGuid = "com.digitalvikings.restedrites";
        public const string ModName = "RestedRites";
        public const string ModVersion = "0.1.0";

        internal static ManualLogSource Log = null!;
        private Harmony _harmony = null!;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"{ModName} v{ModVersion} initializing...");

            // Initialize configuration
            ModConfig.Init(Config);

            // Initialize PowerCalculator 24x4 table
            RestedRites.Rites.PowerCalculator.Initialize();

            // Initialize VikingLib adapter and pipeline subscriptions
            RestedRites.Services.RestedRitesStatAdapter.Init();

            // Apply Harmony patches
            _harmony = new Harmony(ModGuid);
            _harmony.PatchAll();

            // Hook server sync events managed by Jotunn
            HookJotunnSync();

            Log.LogInfo($"{ModName} v{ModVersion} loaded successfully.");
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private void HookJotunnSync()
        {
            Jotunn.Managers.SynchronizationManager.OnConfigurationSynchronized += (sender, args) =>
            {
                Log.LogInfo("Server configuration synchronized successfully. Reloading power table and reapplying active Rested Rites modifiers...");
                RestedRites.Rites.PowerCalculator.ReloadTable();
                SERestedPatch.ReapplyCustomModifiers(Player.m_localPlayer);
            };
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}
