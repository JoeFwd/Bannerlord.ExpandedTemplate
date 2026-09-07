using System;
using System.Linq;
using NpcEquipmentSet = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.EquipmentSet;
using RosterEquipmentSet =
    Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters.EquipmentSet;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;

public class EquipmentSetFlagMapper
{
    public bool IsMatching(RosterEquipmentSet set, NpcEquipmentSet characterSet)
    {
        bool allFlagsFalseOnSet = AllFalse(set.IsCivilian, set.IsSiege, set.IsBattle, set.IsStealth);
        bool allFlagsFalseOnCharacter = AllFalse(characterSet.ResolvedIsCivilian, characterSet.ResolvedIsSiege,
            characterSet.ResolvedIsBattle, characterSet.ResolvedIsStealth);
        bool usesEquipmentType = !string.IsNullOrWhiteSpace(characterSet.EquipmentType) ||
                                 !string.IsNullOrWhiteSpace(characterSet.EquipmentTypes);

        return MatchesFlag(characterSet.ResolvedIsCivilian, set.IsCivilian, usesEquipmentType) ||
               MatchesFlag(characterSet.ResolvedIsSiege, set.IsSiege, usesEquipmentType) ||
               MatchesFlag(characterSet.ResolvedIsBattle, set.IsBattle, usesEquipmentType) ||
               MatchesFlag(characterSet.ResolvedIsStealth, set.IsStealth, usesEquipmentType) ||
               (allFlagsFalseOnSet && allFlagsFalseOnCharacter) ||
               (usesEquipmentType &&
                set.IsCivilian == null &&
                set.IsSiege == null &&
                set.IsBattle == null &&
                set.IsStealth == null &&
                bool.TrueString.Equals(characterSet.ResolvedIsBattle, StringComparison.OrdinalIgnoreCase));
    }

    private static bool AllFalse(params string?[] flags)
    {
        return flags.All(flag => !bool.TrueString.Equals(flag, StringComparison.OrdinalIgnoreCase));
    }

    private static bool MatchesFlag(string? characterFlag, string? setFlag, bool usesEquipmentType)
    {
        return bool.TrueString.Equals(characterFlag, StringComparison.OrdinalIgnoreCase) &&
               (bool.TrueString.Equals(setFlag, StringComparison.OrdinalIgnoreCase) ||
                (!usesEquipmentType && setFlag == null));
    }
}
