using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using Equipment = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.Equipment;
using EquipmentRoster = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.EquipmentRoster;
using NpcEquipmentSet = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.EquipmentSet;
using RosterEquipmentSet =
    Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters.EquipmentSet;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;

public class NpcCharacterMapper : INpcCharacterMapper
{
    private readonly IEquipmentRosterRepository _equipmentRosterRepository;
    private readonly EquipmentSetFlagMapper _equipmentSetFlagMapper;
    private readonly IEquipmentSetMapper _equipmentSetMapper;
    private readonly ILogger _logger;

    public NpcCharacterMapper(
        IEquipmentRosterRepository equipmentRosterRepository,
        IEquipmentSetMapper equipmentSetMapper,
        EquipmentSetFlagMapper equipmentSetFlagMapper,
        ILoggerFactory loggerFactory)
    {
        _equipmentRosterRepository = equipmentRosterRepository;
        _equipmentSetMapper = equipmentSetMapper;
        _equipmentSetFlagMapper = equipmentSetFlagMapper;
        _logger = loggerFactory.CreateLogger<NpcCharacterMapper>();
    }

    public IList<EquipmentRoster> MapToEquipmentRosters(NpcCharacter npcCharacter)
    {
        var allEquipmentRosters = _equipmentRosterRepository.GetEquipmentRosters().EquipmentRoster;
        var equipmentRosters = new List<EquipmentRoster>();

        foreach (var characterSet in npcCharacter.Equipments.EquipmentSet)
        {
            var matchingRoster = allEquipmentRosters
                .FirstOrDefault(equipmentRoster => equipmentRoster?.Id == characterSet.Id);

            if (matchingRoster == null)
            {
                _logger.Warn($"EquipmentSet with id {characterSet.Id} not found for character {npcCharacter.Id}");
                continue;
            }

            var mappedRosters = matchingRoster.EquipmentSet
                .Where(set => _equipmentSetFlagMapper.IsMatching(set, characterSet))
                .Select(_equipmentSetMapper.MapToEquipmentRoster);

            equipmentRosters.AddRange(mappedRosters);
        }

        var equipmentOverride = npcCharacter.Equipments.Equipment
            .Where(e => !string.IsNullOrWhiteSpace(e.Slot))
            .ToDictionary(equipment => equipment.Slot!, equipment => equipment);

        return npcCharacter.Equipments.EquipmentRoster
            .Where(equipmentRoster => equipmentRoster.Equipment.Count > 0)
            .Concat(equipmentRosters)
            .Select(roster => roster with
            {
                Equipment = OverrideEquipment(roster.Equipment, equipmentOverride)
            })
            .ToList();
    }

    private List<Equipment> OverrideEquipment(List<Equipment> original, IDictionary<string, Equipment> overrides)
    {
        var result = original
            .Select(e => new Equipment
            {
                Id = overrides.TryGetValue(e.Slot, out var overrideEq) ? overrideEq.Id : e.Id,
                Slot = e.Slot
            }).ToList();

        result.AddRange(overrides.Values
            .Where(o => result.All(e => e.Slot != o.Slot)));

        return result;
    }

}
