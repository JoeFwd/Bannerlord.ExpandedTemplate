using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.EquipmentRosters;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Models;

public class EquipmentRostersShould
{
    [Test]
    public void EqualWithSameContent()
    {
        var leftNpcCharacters = new EquipmentRosters
        {
            EquipmentRoster = new List<EquipmentRoster>
            {
                CreateEquipmentRoster("1"),
                CreateEquipmentRoster("2"),
                CreateEquipmentRoster(null)
            }
        };

        var rightNpcCharacters = new EquipmentRosters
        {
            EquipmentRoster = new List<EquipmentRoster>
            {
                CreateEquipmentRoster("1"),
                CreateEquipmentRoster("2"),
                CreateEquipmentRoster(null)
            }
        };

        var isEqual = leftNpcCharacters.Equals(rightNpcCharacters);

        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void NotEqualWithSameContent()
    {
        var leftNpcCharacters = new EquipmentRosters
        {
            EquipmentRoster = new List<EquipmentRoster>
            {
                CreateEquipmentRoster("1"),
                CreateEquipmentRoster("2")
            }
        };

        var rightNpcCharacters = new EquipmentRosters
        {
            EquipmentRoster = new List<EquipmentRoster>
            {
                CreateEquipmentRoster("1"),
                CreateEquipmentRoster(null)
            }
        };
        var isEqual = leftNpcCharacters.Equals(rightNpcCharacters);

        Assert.That(isEqual, Is.False);
    }

    private EquipmentRoster CreateEquipmentRoster(string id)
    {
        return new EquipmentRoster
        {
            Id = id,
            EquipmentSet = new List<EquipmentSet>
            {
                new()
                {
                    Equipment = new List<Equipment>
                    {
                        new()
                        {
                            Id = id,
                            Slot = id
                        }
                    },
                    Pool = id,
                    IsBattle = id,
                    IsCivilian = id,
                    IsSiege = id
                }
            }
        };
    }
}