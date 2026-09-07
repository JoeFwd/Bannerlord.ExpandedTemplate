using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Stealth;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Infrastructure.Tests.EquipmentPool.Get.Stealth;

public class TroopStealthEquipmentPoolProviderShould
{
    private const string TroopId = "troop id";
    private Mock<IEquipmentPoolsProvider> _equipmentPoolsProvider;
    private TroopStealthEquipmentPoolProvider _troopEquipmentPoolProvider;

    [SetUp]
    public void SetUp()
    {
        _equipmentPoolsProvider = new Mock<IEquipmentPoolsProvider>();
        var loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory.Setup(factory => factory.CreateLogger<TroopStealthEquipmentPoolProvider>())
            .Returns(new Mock<ILogger>().Object);
        _troopEquipmentPoolProvider = new TroopStealthEquipmentPoolProvider(loggerFactory.Object,
            _equipmentPoolsProvider.Object);
    }

    [TestCase("")]
    [TestCase(null)]
    public void ReturnNoEquipmentPoolsIfTroopIdIsEmpty(string troopId)
    {
        Assert.That(_troopEquipmentPoolProvider.GetStealthTroopEquipmentPools(troopId), Is.Empty);
    }

    [Test]
    public void ReturnNoEquipmentPoolsIfTroopIdDoesNotExist()
    {
        _equipmentPoolsProvider.Setup(provider => provider.GetEquipmentPoolsByCharacterId())
            .Returns(new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>());

        Assert.That(_troopEquipmentPoolProvider.GetStealthTroopEquipmentPools(TroopId), Is.Empty);
    }

    [Test]
    public void ReturnStealthEquipmentPools()
    {
        var equipmentPools = new List<Domain.EquipmentPool.Model.EquipmentPool>
        {
            new(new List<Equipment>(), 0)
        };
        _equipmentPoolsProvider.Setup(provider => provider.GetEquipmentPoolsByCharacterId()).Returns(
            new Dictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>>
            {
                { TroopId, equipmentPools }
            });

        Assert.That(_troopEquipmentPoolProvider.GetStealthTroopEquipmentPools(TroopId), Is.EqualTo(equipmentPools));
    }
}
