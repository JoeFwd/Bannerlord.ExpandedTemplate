using System;
using System.Linq;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models;

/// <summary>
///     Resolves the legacy battle/siege/civilian boolean attributes together with the newer
///     <c>equipmentType</c> (single value) and <c>equipmentTypes</c> (semicolon-separated list)
///     attributes into a single effective flag value.
/// </summary>
internal static class EquipmentTypeFlags
{
    public const string Battle = "Battle";
    public const string Siege = "Siege";
    public const string Civilian = "Civilian";
    public const string Stealth = "Stealth";

    public static string? Resolve(string? legacyFlag, string? equipmentType, string? equipmentTypes, string type,
        bool defaultUnspecifiedTypesToFalse = false)
    {
        if (bool.TrueString.Equals(legacyFlag, StringComparison.OrdinalIgnoreCase))
            return legacyFlag;

        if (string.Equals(equipmentType, type, StringComparison.OrdinalIgnoreCase))
            return bool.TrueString;

        if (equipmentTypes != null && equipmentTypes.Split(';')
                .Any(t => string.Equals(t.Trim(), type, StringComparison.OrdinalIgnoreCase)))
            return bool.TrueString;

        return defaultUnspecifiedTypesToFalse && UsesEquipmentType(equipmentType, equipmentTypes)
            ? bool.FalseString
            : legacyFlag;
    }

    private static bool UsesEquipmentType(string? equipmentType, string? equipmentTypes)
    {
        return !string.IsNullOrWhiteSpace(equipmentType) || !string.IsNullOrWhiteSpace(equipmentTypes);
    }
}
