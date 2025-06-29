using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Civilian;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.Get.Civilian;

public class TroopCivilianEquipmentPoolProviderShould
{
    private const string FirstTroopId = "first unrelevant troop id";
    private const string SecondTroopId = "second unrelevant troop id";

    private Mock<ILoggerFactory> _loggerFactory;
    private Mock<IEquipmentPoolsProvider> _equipmentPoolsProvider;
    private TroopCivilianEquipmentPoolProvider _troopEquipmentPoolProvider;

    [SetUp]
    public void Setup()
    {
        _equipmentPoolsProvider = new Mock<IEquipmentPoolsProvider>();
        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(factory => factory.CreateLogger<TroopCivilianEquipmentPoolProvider>())
            .Returns(new Mock<ILogger>().Object);

        _troopEquipmentPoolProvider =
            new TroopCivilianEquipmentPoolProvider(_loggerFactory.Object, _equipmentPoolsProvider.Object);
    }


    [Test]
    public void ReturnNoEquipmentPoolsIfTroopIdIsEmpty()
    {
        var actualEquipmentPools = _troopEquipmentPoolProvider.GetCivilianTroopEquipmentPools("");

        Assert.That(actualEquipmentPools, Is.Empty);
    }

    [Test]
    public void ReturnNoEquipmentPoolsIfTroopIdIsNull()
    {
        var actualEquipmentPools = _troopEquipmentPoolProvider.GetCivilianTroopEquipmentPools(null!);

        Assert.That(actualEquipmentPools, Is.Empty);
    }

    [Test]
    public void ReturnNoEquipmentPoolsIfTroopIdDoesNotExist()
    {
        _equipmentPoolsProvider.Setup(provider => provider.GetEquipmentPoolsByCharacterId())
            .Returns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>());

        var actualEquipmentPools = _troopEquipmentPoolProvider.GetCivilianTroopEquipmentPools(FirstTroopId);

        Assert.That(actualEquipmentPools, Is.Empty);
    }

    [Test]
    public void ReturnBattleEquipmentPools()
    {
        var equipmentPools1 = new List<Domain.EquipmentPool.Model.EquipmentPool> { new(new List<Equipment>(), 0) };
        var equipmentPools2 = new List<Domain.EquipmentPool.Model.EquipmentPool> { new(new List<Equipment>(), 1) };
        _equipmentPoolsProvider.Setup(provider => provider.GetEquipmentPoolsByCharacterId()).Returns(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                { FirstTroopId, equipmentPools1 },
                { SecondTroopId, equipmentPools2 }
            });

        var actualEquipmentPools = _troopEquipmentPoolProvider.GetCivilianTroopEquipmentPools(FirstTroopId);

        Assert.That(actualEquipmentPools, Is.EqualTo(equipmentPools1));
    }
}