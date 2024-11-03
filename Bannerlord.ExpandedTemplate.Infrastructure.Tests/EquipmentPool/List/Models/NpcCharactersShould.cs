using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Models.NpcCharacters;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.List.Models;

public class NpcCharactersShould
{
    [Test]
    public void EqualWithSameContent()
    {
        var leftNpcCharacters = new NpcCharacters
        {
            NpcCharacter = new List<NpcCharacter>
            {
                CreateNpcCharacter("1"),
                CreateNpcCharacter("2"),
                CreateNpcCharacter(null)
            }
        };

        var rightNpcCharacters = new NpcCharacters
        {
            NpcCharacter = new List<NpcCharacter>
            {
                CreateNpcCharacter("1"),
                CreateNpcCharacter("2"),
                CreateNpcCharacter(null)
            }
        };

        var isEqual = leftNpcCharacters.Equals(rightNpcCharacters);

        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void NotEqualWithSameContent()
    {
        var leftNpcCharacters = new NpcCharacters
        {
            NpcCharacter = new List<NpcCharacter>
            {
                CreateNpcCharacter("1"),
                CreateNpcCharacter("2")
            }
        };

        var rightNpcCharacters = new NpcCharacters
        {
            NpcCharacter = new List<NpcCharacter>
            {
                CreateNpcCharacter("1"),
                CreateNpcCharacter(null)
            }
        };

        var isEqual = leftNpcCharacters.Equals(rightNpcCharacters);

        Assert.That(isEqual, Is.False);
    }

    private NpcCharacter CreateNpcCharacter(string id)
    {
        return new NpcCharacter
        {
            Id = id,
            Equipments = new Equipments
            {
                Equipment = new List<Equipment>
                {
                    new()
                    {
                        Id = id,
                        Slot = id
                    }
                },
                EquipmentRoster = new List<EquipmentRoster>
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
                },
                EquipmentSet = new List<EquipmentSet>
                {
                    new()
                    {
                        Id = id,
                        IsCivilian = id,
                        IsSiege = id,
                        IsBattle = id,
                        Pool = id
                    }
                }
            }
        };
    }
}