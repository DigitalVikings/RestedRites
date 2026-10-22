using System;
using System.Collections.Generic;
using VikingLib.Stats;

namespace RestedRites.Rites
{
    public delegate IEnumerable<StatModifier> SetBonusModifierFactory(Player player, object owner);

    public sealed class TrophySet
    {
        public SetId Id { get; }
        public string Name { get; }
        public TrophyId[] Members { get; }
        public string? TwoPieceBonus { get; }
        public string? ThreePieceBonus { get; }
        public string? FourPieceBonus { get; }

        public SetBonusModifierFactory? TwoPieceModifiers { get; }
        public SetBonusModifierFactory? ThreePieceModifiers { get; }
        public SetBonusModifierFactory? FourPieceModifiers { get; }

        public TrophySet(
            SetId id,
            string name,
            TrophyId[] members,
            string? twoPieceBonus = null,
            SetBonusModifierFactory? twoPieceModifiers = null,
            string? threePieceBonus = null,
            SetBonusModifierFactory? threePieceModifiers = null,
            string? fourPieceBonus = null,
            SetBonusModifierFactory? fourPieceModifiers = null)
        {
            Id = id;
            Name = name;
            Members = members;
            TwoPieceBonus = twoPieceBonus;
            TwoPieceModifiers = twoPieceModifiers;
            ThreePieceBonus = threePieceBonus;
            ThreePieceModifiers = threePieceModifiers;
            FourPieceBonus = fourPieceBonus;
            FourPieceModifiers = fourPieceModifiers;
        }

        public bool Contains(TrophyId trophyId)
        {
            for (int i = 0; i < Members.Length; i++)
            {
                if (Members[i] == trophyId) return true;
            }
            return false;
        }

        public bool HasBonus(int activeCount, int pieceTier)
        {
            return activeCount >= pieceTier;
        }

        public IEnumerable<StatModifier> CreateModifiers(Player player, int activeCount, object owner)
        {
            if (activeCount >= 2 && TwoPieceModifiers != null)
            {
                foreach (var mod in TwoPieceModifiers(player, owner)) yield return mod;
            }
            if (activeCount >= 3 && ThreePieceModifiers != null)
            {
                foreach (var mod in ThreePieceModifiers(player, owner)) yield return mod;
            }
            if (activeCount >= 4 && FourPieceModifiers != null)
            {
                foreach (var mod in FourPieceModifiers(player, owner)) yield return mod;
            }
        }
    }
}
