using System;
using System.Collections.Generic;
using RestedRites.Configuration;
using UnityEngine;

namespace RestedRites.Services
{
    public static class FeedbackManager
    {
        public static GameObject? SpawnEffect(string primaryPrefab, Vector3 position, Quaternion rotation, params string[] fallbacks)
        {
            return SpawnEffect(primaryPrefab, position, rotation, 0f, fallbacks);
        }

        public static GameObject? SpawnEffect(string primaryPrefab, Vector3 position, Quaternion rotation, float durationSeconds, params string[] fallbacks)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || ZNetScene.instance == null)
            {
                return null;
            }

            var prefab = ZNetScene.instance.GetPrefab(primaryPrefab);
            if (prefab == null && fallbacks != null)
            {
                for (int i = 0; i < fallbacks.Length; i++)
                {
                    prefab = ZNetScene.instance.GetPrefab(fallbacks[i]);
                    if (prefab != null) break;
                }
            }

            if (prefab != null)
            {
                var instance = UnityEngine.Object.Instantiate(prefab, position, rotation);
                if (instance != null)
                {
                    if (durationSeconds > 0.01f)
                    {
                        foreach (var ps in instance.GetComponentsInChildren<ParticleSystem>())
                        {
                            var main = ps.main;
                            float speedMultiplier = Mathf.Max(1f, 1.5f / durationSeconds);
                            main.simulationSpeed = speedMultiplier;
                            main.maxParticles = Math.Min(main.maxParticles, 15);
                            var emission = ps.emission;
                            emission.rateOverTimeMultiplier *= 0.4f;
                        }

                        var td = instance.GetComponent<TimedDestruction>();
                        if (td != null)
                        {
                            td.CancelInvoke();
                        }
                        UnityEngine.Object.Destroy(instance, durationSeconds);
                    }
                    else if (instance.GetComponent<TimedDestruction>() == null)
                    {
                        UnityEngine.Object.Destroy(instance, 5f);
                    }
                }
                return instance;
            }

            return null;
        }

        private static readonly HarmonyLib.AccessTools.FieldRef<ItemStand, GameObject> ItemStandVisualItemRef =
            HarmonyLib.AccessTools.FieldRefAccess<ItemStand, GameObject>("m_visualItem");

        public static void TriggerAttunement(ItemStand stand, string riteName)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || stand == null) return;

            Vector3 standPosition = stand.transform.position;

            // 1. Trophy Emission Flash (pulse on the mounted trophy model)
            // Look on m_visualItem, or search m_attachOther / stand children if m_visualItem is a wrapper or not directly set
            GameObject? targetObj = ItemStandVisualItemRef(stand);
            if (targetObj == null && stand.m_attachOther != null && stand.m_attachOther.childCount > 0)
            {
                targetObj = stand.m_attachOther.GetChild(0).gameObject;
            }
            if (targetObj == null)
            {
                targetObj = stand.gameObject;
            }

            if (targetObj != null)
            {
                var flasher = targetObj.GetComponent<TrophyEmissionFlasher>();
                if (flasher == null)
                {
                    flasher = targetObj.AddComponent<TrophyEmissionFlasher>();
                }
                flasher.Flash(stand, new Color(1.0f, 0.8f, 0.35f), duration: 0.8f);
            }

            // 2. Play subtle, low, organic sound via ItemStand native effects or lore chime
            if (stand.m_effects != null && stand.m_effects.HasEffects())
            {
                stand.m_effects.Create(standPosition, Quaternion.identity);
            }
            else
            {
                SpawnEffect("sfx_lore", standPosition, Quaternion.identity, "sfx_build_hammer_stone");
            }

            // 3. Floating feedback text (using ASCII-compatible symbols supported by Valheim-AveriaSansLibre)
            if (ModConfig.EnableCombatFloatingText.Value && DamageText.instance != null)
            {
                DamageText.instance.ShowText(DamageText.TextType.Bonus, standPosition + Vector3.up * 0.6f, $"* Attuned: {riteName} *", false);
            }
        }

        private class TrophyEmissionFlasher : MonoBehaviour
        {
            private Renderer[] _renderers = Array.Empty<Renderer>();
            private MaterialPropertyBlock _propBlock = null!;
            private Light? _flashLight;
            private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
            private static readonly int ColorId = Shader.PropertyToID("_Color");
            private Color _targetColor;
            private float _duration;
            private float _elapsed;

            private void Awake()
            {
                _propBlock = new MaterialPropertyBlock();
                DiscoverRenderers();
            }

            public void DiscoverRenderers(ItemStand? stand = null)
            {
                var found = new List<Renderer>();

                // Check self and children
                found.AddRange(GetComponentsInChildren<Renderer>(true));

                // If attached to an ItemStand, also check attach points in case trophy model was attached elsewhere
                if (stand != null)
                {
                    if (stand.m_attachOther != null)
                    {
                        found.AddRange(stand.m_attachOther.GetComponentsInChildren<Renderer>(true));
                    }
                    if (stand.m_dropSpawnPoint != null)
                    {
                        found.AddRange(stand.m_dropSpawnPoint.GetComponentsInChildren<Renderer>(true));
                    }
                }

                // Filter out ItemStand's own frame/wood base if stand was passed
                if (stand != null)
                {
                    var standRenderers = stand.GetComponents<Renderer>();
                    found.RemoveAll(r => Array.IndexOf(standRenderers, r) >= 0);
                }

                _renderers = found.ToArray();

                // Enable EMISSION keyword on materials where needed so the shader respects emission
                for (int i = 0; i < _renderers.Length; i++)
                {
                    var r = _renderers[i];
                    if (r != null)
                    {
                        var mats = r.sharedMaterials;
                        for (int m = 0; m < mats.Length; m++)
                        {
                            var mat = mats[m];
                            if (mat != null)
                            {
                                mat.EnableKeyword("_EMISSION");
                            }
                        }
                    }
                }
            }

            public void Flash(ItemStand stand, Color color, float duration)
            {
                _targetColor = color;
                _duration = Mathf.Max(0.1f, duration);
                _elapsed = 0f;

                DiscoverRenderers(stand);

                // Add a gentle, subtle point light that illuminates the trophy model directly
                if (_flashLight == null)
                {
                    _flashLight = gameObject.GetComponent<Light>();
                    if (_flashLight == null)
                    {
                        _flashLight = gameObject.AddComponent<Light>();
                    }
                    _flashLight.type = LightType.Point;
                    _flashLight.range = 2.0f;
                    _flashLight.shadows = LightShadows.None;
                }
                _flashLight.color = color;
                _flashLight.intensity = 0f;
                _flashLight.enabled = true;

                enabled = true;
            }

            private void Update()
            {
                _elapsed += Time.deltaTime;
                float progress = _elapsed / _duration;

                if (progress >= 1f)
                {
                    SetIntensity(0f);
                    if (_flashLight != null)
                    {
                        _flashLight.enabled = false;
                    }
                    enabled = false;
                    return;
                }

                // Smooth bell curve pulse: 0 -> 1 -> 0
                float curve = Mathf.Sin(progress * Mathf.PI);
                SetIntensity(curve);

                if (_flashLight != null)
                {
                    _flashLight.intensity = curve * 1.5f;
                }
            }

            private void SetIntensity(float factor)
            {
                if (_renderers == null) return;
                Color col = _targetColor * factor;

                for (int i = 0; i < _renderers.Length; i++)
                {
                    var r = _renderers[i];
                    if (r != null)
                    {
                        r.GetPropertyBlock(_propBlock);
                        _propBlock.SetColor(EmissionColorId, col);
                        r.SetPropertyBlock(_propBlock);
                    }
                }
            }

            private void OnDisable()
            {
                SetIntensity(0f);
                if (_flashLight != null)
                {
                    _flashLight.enabled = false;
                }
            }
        }

        public static void TriggerStaminaBurst(Player player)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || player == null) return;

            // Short stamina mead visual effect without floating text clutter
            SpawnEffect("vfx_StaminaUpgrade", player.transform.position, Quaternion.identity, "vfx_MeadStrength");
            SpawnEffect("sfx_Potion_stamina_Start", player.transform.position, Quaternion.identity);
        }

        public static void TriggerBloodboundStrike(Vector3 hitPoint, Vector3 fallbackPoint = default)
        {
            if (!ModConfig.EnableSensoryFeedback.Value) return;

            Vector3 point = hitPoint != Vector3.zero ? hitPoint : (fallbackPoint != Vector3.zero ? fallbackPoint : Vector3.zero);
            // Critical hit visual effect
            SpawnEffect("fx_crit", point, Quaternion.identity, "vfx_corpse_destruction_hit");
        }

        public static void TriggerCarapaceWard(Player player, Vector3 hitPoint)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || player == null) return;

            Vector3 point = hitPoint != Vector3.zero ? hitPoint : player.GetCenterPoint();
            SpawnEffect("vfx_blocked", point, Quaternion.identity);
            SpawnEffect("fx_StaffShield_Hit", player.transform.position, Quaternion.identity);
            SpawnEffect("sfx_metal_blocked_overlay", point, Quaternion.identity);

            if (ModConfig.EnableCombatFloatingText.Value && DamageText.instance != null)
            {
                DamageText.instance.ShowText(DamageText.TextType.Blocked, player.transform.position + Vector3.up * 0.5f, "CARAPACE WARD (-80%)", true);
            }
        }

        public static void TriggerCarapaceCounter(Character target, float reflectedDamage)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || target == null) return;

            SpawnEffect("fx_StaffShield_Hit", target.transform.position, Quaternion.identity);

            if (ModConfig.EnableCombatFloatingText.Value && DamageText.instance != null)
            {
                DamageText.instance.ShowText(DamageText.TextType.Weak, target.GetHeadPoint() + Vector3.up * 0.3f, reflectedDamage, false);
            }
        }

        public static void TriggerLastStand(Player player, float healAmount)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || player == null) return;

            SpawnEffect("vfx_HealthUpgrade", player.transform.position, Quaternion.identity, "vfx_Potion_health_medium");
            SpawnEffect("sfx_dverger_heal_finish", player.transform.position, Quaternion.identity);

            if (ModConfig.EnableCombatFloatingText.Value && DamageText.instance != null)
            {
                DamageText.instance.ShowText(DamageText.TextType.Heal, player.GetHeadPoint() + Vector3.up * 0.5f, healAmount, true);
            }
        }

        public static void TriggerHeal(Player player, float healAmount)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || player == null) return;

            if (ModConfig.EnableCombatFloatingText.Value && DamageText.instance != null)
            {
                DamageText.instance.ShowText(DamageText.TextType.Heal, player.GetHeadPoint() + Vector3.up * 0.4f, healAmount, true);
            }
        }

        public static void TriggerTarProc(Character? target)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || target == null) return;

            SpawnEffect("vfx_blobtar_hit", target.GetCenterPoint(), Quaternion.identity);
        }

        public static void TriggerFrostProc(Character? target)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || target == null) return;

            SpawnEffect("vfx_Frost", target.GetCenterPoint(), Quaternion.identity);
        }

        public static void TriggerSpeedBoost(Player player)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || player == null) return;

            SpawnEffect("sfx_frozenking_charge_whoosh", player.transform.position, Quaternion.identity, "sfx_club_swing");
        }


        public static void TriggerBrennaFireburst(Vector3 position, float damage, Player attacker)
        {
            if (!ModConfig.EnableSensoryFeedback.Value) return;

            SpawnEffect("vfx_FireballHit", position, Quaternion.identity, "vfx_corpse_destruction_hit");

            if (damage > 0.1f)
            {
                var colliders = Physics.OverlapSphere(position, 4.0f);
                var damaged = new HashSet<Character>();
                for (int i = 0; i < colliders.Length; i++)
                {
                    var col = colliders[i];
                    if (col == null) continue;
                    var target = col.GetComponentInParent<Character>();
                    if (target != null && target != attacker && !target.IsPlayer() && !target.IsTamed() && damaged.Add(target))
                    {
                        HitData aoeHit = new HitData();
                        aoeHit.m_damage.m_fire = damage;
                        aoeHit.m_point = target.GetCenterPoint();
                        aoeHit.m_dir = (target.transform.position - position).normalized;
                        aoeHit.SetAttacker(attacker);
                        target.Damage(aoeHit);
                    }
                }
            }
        }

        public static void TriggerGeirrhafaFrostFog(Vector3 position, Player player)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || player == null) return;

            SpawnEffect("vfx_BombBlob_explode_frost", position, Quaternion.identity, "vfx_Frost");

            var colliders = Physics.OverlapSphere(position, 3.5f);
            var affected = new HashSet<Character>();
            for (int i = 0; i < colliders.Length; i++)
            {
                var col = colliders[i];
                if (col == null) continue;
                var target = col.GetComponentInParent<Character>();
                if (target != null && target != player && !target.IsPlayer() && !target.IsTamed() && affected.Add(target))
                {
                    target.GetSEMan()?.AddStatusEffect(StringExtensionMethods.GetStableHashCode("Frost"), resetTime: true);
                }
            }
        }

        public static void TriggerShamanicWard(Player player, string debuffName)
        {
            if (!ModConfig.EnableSensoryFeedback.Value || player == null) return;

            SpawnEffect("fx_StaffShield_Hit", player.transform.position, Quaternion.identity);
            SpawnEffect("sfx_lore", player.transform.position, Quaternion.identity);

            if (ModConfig.EnableCombatFloatingText.Value && DamageText.instance != null)
            {
                DamageText.instance.ShowText(DamageText.TextType.Bonus, player.GetHeadPoint() + Vector3.up * 0.5f, $"WARDED: {debuffName}", true);
            }
        }
    }
}
