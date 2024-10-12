﻿using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;
using NUnit.Framework;
using EquipmentRoster = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.EquipmentRoster;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Mappers;

public class EquipmentSetMapperShould
{
    private const string Pool = "irrelevant_pool_id";
    private const string IsCivilian = "irrelevant_civilian_flag";
    private const string IsSiege = "irrelevant_siege_flag";
    private const string Slot1 = "irrelevant_slot1";
    private const string Slot2 = "irrelevant_slot2";
    private const string EquipmentId1 = "irrelevant_equipment_id1";
    private const string EquipmentId2 = "irrelevant_equipment_id2";

    private IEquipmentSetMapper _equipmentSetMapper;

    [SetUp]
    public void SetUp()
    {
        _equipmentSetMapper = new EquipmentSetMapper();
    }

    [Test]
    public void ThrowsArgumentNullExceptionWhenEquipmentSetIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _equipmentSetMapper.MapToEquipmentRoster(null));
    }

    [Test]
    public void MapEquipmentSet()
    {
        EquipmentSet equipmentSet = new EquipmentSet
        {
            Pool = Pool,
            IsCivilian = IsCivilian,
            IsSiege = IsSiege,
            Equipment = new List<Equipment>
            {
                new() { Slot = Slot1, Id = EquipmentId1 },
                new() { Slot = Slot2, Id = EquipmentId2 }
            }
        };

        EquipmentRoster equipmentRoster = _equipmentSetMapper.MapToEquipmentRoster(equipmentSet);

        Assert.That(equipmentRoster, Is.EqualTo(new EquipmentRoster
        {
            Pool = Pool,
            IsCivilian = IsCivilian,
            IsSiege = IsSiege,
            Equipment = new List<Infrastructure.EquipmentPool.List.Models.NpcCharacters.Equipment>
            {
                new() { Slot = Slot1, Id = EquipmentId1 },
                new() { Slot = Slot2, Id = EquipmentId2 }
            }
        }));
    }

    [Test]
    public void MapEquipmentSetWithEmptyValues()
    {
        EquipmentSet equipmentSet = new EquipmentSet
        {
            Pool = null,
            IsCivilian = null,
            IsSiege = null,
            Equipment = null
        };

        EquipmentRoster equipmentRoster = _equipmentSetMapper.MapToEquipmentRoster(equipmentSet);

        Assert.That(equipmentRoster, Is.EqualTo(new EquipmentRoster
        {
            Pool = null,
            IsCivilian = null,
            IsSiege = null,
            Equipment = new List<Infrastructure.EquipmentPool.List.Models.NpcCharacters.Equipment>()
        }));
    }
}