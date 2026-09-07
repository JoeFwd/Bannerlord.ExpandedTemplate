using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;
using Moq;
using NUnit.Framework;
using EquipmentPoolModel = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model.EquipmentPool;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.Get;

public class TroopEquipmentPoolProviderShould
{
    [TestCase("")]
    [TestCase(null)]
    public void ReturnNoEquipmentPoolsForAnEmptyTroopId(string? troopId)
    {
        var provider = new TroopEquipmentPoolProvider(new Mock<ILogger>().Object,
            new Mock<IEquipmentPoolsProvider>().Object, "battle");

        IList<EquipmentPoolModel> equipmentPools = provider.GetTroopEquipmentPools(troopId!);

        Assert.That(equipmentPools, Is.Empty);
    }

    [Test]
    public void ReturnNoEquipmentPoolsForAnUnknownTroopId()
    {
        var equipmentPoolsProvider = new Mock<IEquipmentPoolsProvider>();
        equipmentPoolsProvider.Setup(provider => provider.GetEquipmentPoolsByCharacterId())
            .Returns(new Dictionary<string, IList<EquipmentPoolModel>>());
        var provider = new TroopEquipmentPoolProvider(new Mock<ILogger>().Object, equipmentPoolsProvider.Object, "siege");

        IList<EquipmentPoolModel> equipmentPools = provider.GetTroopEquipmentPools("unknown");

        Assert.That(equipmentPools, Is.Empty);
    }

    [Test]
    public void ReturnEquipmentPoolsForAKnownTroopId()
    {
        IList<EquipmentPoolModel> expected = new List<EquipmentPoolModel>
        {
            new(new List<Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model.Equipment>(), 0)
        };
        var equipmentPoolsProvider = new Mock<IEquipmentPoolsProvider>();
        equipmentPoolsProvider.Setup(provider => provider.GetEquipmentPoolsByCharacterId())
            .Returns(new Dictionary<string, IList<EquipmentPoolModel>> { ["troop"] = expected });
        var provider = new TroopEquipmentPoolProvider(new Mock<ILogger>().Object, equipmentPoolsProvider.Object, "stealth");

        IList<EquipmentPoolModel> equipmentPools = provider.GetTroopEquipmentPools("troop");

        Assert.That(equipmentPools, Is.EqualTo(expected));
    }
}
