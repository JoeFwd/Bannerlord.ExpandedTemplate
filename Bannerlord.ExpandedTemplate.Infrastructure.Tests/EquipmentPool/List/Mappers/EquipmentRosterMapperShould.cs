using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using NUnit.Framework;
using Equipment = Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters.Equipment;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Mappers;

public class EquipmentRosterMapperShould
{
    private IEquipmentRosterMapper _equipmentRosterMapper;

    [SetUp]
    public void SetUp()
    {
        _equipmentRosterMapper = new EquipmentRosterMapper();
    }

    [Test]
    public void MapsToEquipmentPool()
    {
        var equipmentRoster = new EquipmentRoster
        {
            Pool = "0",
            Equipment = new List<Equipment>
            {
                new()
                {
                    Slot = "irrevelant slot",
                    Id = "irrevelant id"
                },
                new()
                {
                    Slot = "irrevelant slot",
                    Id = "irrevelant id"
                }
            }
        };

        var equipmentPool = _equipmentRosterMapper.MapToEquipmentPool(equipmentRoster);

        Assert.That(equipmentPool,
            Is.EqualTo(new Domain.EquipmentPool.Model.EquipmentPool(new List<Domain.EquipmentPool.Model.Equipment>
                {
                    new(new List<EquipmentSlot>
                    {
                        new("irrevelant slot", "irrevelant id"),
                        new("irrevelant slot", "irrevelant id")
                    })
                },
                0)));
    }
}