using System;
using System.Collections.Generic;
using VikingLib.Stats;

namespace RestedRites.Rites
{
    public delegate IEnumerable<StatModifier> RiteModifierFactory(Player player, float power, object owner);

    public class TrophyRite
    {
        public TrophyId Id { get; }
        public string PrefabName { get; }
        public string[] AlternatePrefabs { get; }
        public string DisplayName { get; }
        public string CreatureName { get; }
        public string Category { get; }
        public float BaseMagnitude { get; }
        public string StatUnit { get; }
        public string DescriptionFormat { get; }
        public RiteModifierFactory? ModifierFactory { get; }

        public TrophyRite(
            TrophyId id,
            string prefabName,
            string displayName,
            string creatureName,
            string category,
            float baseMagnitude,
            string statUnit,
            string descriptionFormat,
            RiteModifierFactory? modifierFactory = null,
            params string[] alternatePrefabs)
        {
            Id = id;
            PrefabName = prefabName;
            DisplayName = displayName;
            CreatureName = creatureName;
            Category = category;
            BaseMagnitude = baseMagnitude;
            StatUnit = statUnit;
            DescriptionFormat = descriptionFormat;
            ModifierFactory = modifierFactory;
            AlternatePrefabs = alternatePrefabs ?? Array.Empty<string>();
        }

        public float GetCalculatedMagnitude(int playerComfort)
        {
            return PowerCalculator.CalculatePower(BaseMagnitude, playerComfort);
        }

        public string GetFormattedDescription(int playerComfort)
        {
            if (Id == TrophyId.SeekerSoldier)
            {
                float p = GetCalculatedMagnitude(playerComfort);
                float cd = p > 0.01f ? 90f / p : 90f;
                return string.Format(DescriptionFormat, cd.ToString("F0"), StatUnit);
            }
            if (Id == TrophyId.Abomination)
            {
                return string.Format(DescriptionFormat, "1", StatUnit);
            }
            float scaled = GetCalculatedMagnitude(playerComfort);
            string formattedValue = scaled >= 10f ? Math.Round(scaled).ToString("F0") : scaled.ToString("F1");
            return string.Format(DescriptionFormat, formattedValue, StatUnit);
        }

        public string GetBaseDescription()
        {
            if (Id == TrophyId.SeekerSoldier)
            {
                return string.Format(DescriptionFormat, "90", StatUnit);
            }
            float val = BaseMagnitude;
            string formattedValue = val >= 10f ? Math.Round(val).ToString("F0") : val.ToString("F1");
            return string.Format(DescriptionFormat, formattedValue, StatUnit);
        }

        public IEnumerable<StatModifier> CreateModifiers(Player player, int playerComfort, object owner)
        {
            if (ModifierFactory == null) yield break;
            float power = GetCalculatedMagnitude(playerComfort);
            foreach (var mod in ModifierFactory(player, power, owner))
            {
                yield return mod;
            }
        }
    }
}
