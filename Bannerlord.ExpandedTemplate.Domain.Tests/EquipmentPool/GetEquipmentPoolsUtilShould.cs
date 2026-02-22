using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Domain.Tests.EquipmentPool;

public class GetEquipmentPoolsUtilShould
{
    private const string TroopId = "unrelevant troop id";
    private readonly Domain.EquipmentPool.Model.EquipmentPool _equipmentPool = CreateEquipmentPool();
    private Mock<ITroopBattleEquipmentProvider> _troopBattleEquipmentProvider;
    private Mock<ITroopSiegeEquipmentProvider> _troopSiegeEquipmentProvider;
    private Mock<ITroopCivilianEquipmentProvider> _troopCivilianEquipmentProvider;
    private Mock<IEncounterTypeProvider> _encounterTypeProvider;
    private Mock<ILoggerFactory> _loggerFactory;
    private Mock<ILogger> _logger;
    private GetEquipmentPoolsUtil _getEquipmentPoolsUtil;

    [SetUp]
    public void Setup()
    {
        _troopBattleEquipmentProvider = new Mock<ITroopBattleEquipmentProvider>();
        _troopSiegeEquipmentProvider = new Mock<ITroopSiegeEquipmentProvider>();
        _troopCivilianEquipmentProvider = new Mock<ITroopCivilianEquipmentProvider>();
        _encounterTypeProvider = new Mock<IEncounterTypeProvider>();
        _logger = new Mock<ILogger>();
        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(factory => factory.CreateLogger<GetEquipmentPoolsUtil>())
            .Returns(_logger.Object);
        _getEquipmentPoolsUtil = new GetEquipmentPoolsUtil(_encounterTypeProvider.Object,
            _troopBattleEquipmentProvider.Object, _troopSiegeEquipmentProvider.Object,
            _troopCivilianEquipmentProvider.Object, _loggerFactory.Object);
    }

    [Test]
    public void ReturnBattleEquipmentPoolsWhenEncounterIsBattle()
    {
        var equipmentPools = new List<Domain.EquipmentPool.Model.EquipmentPool> { _equipmentPool };
        _encounterTypeProvider.Setup(encounterProvider => encounterProvider.GetEncounterType())
            .Returns(EncounterType.Battle);
        _troopBattleEquipmentProvider.Setup(listBattleEquipment => listBattleEquipment
                .GetBattleTroopEquipmentPools(TroopId))
            .Returns(equipmentPools);

        var equipment = _getEquipmentPoolsUtil.GetEquipmentPools(TroopId);

        Assert.That(equipment, Is.EqualTo(equipmentPools));
    }

    [Test]
    public void ReturnSiegeEquipmentPoolsWhenEncounterIsSiege()
    {
        var equipmentPools = new List<Domain.EquipmentPool.Model.EquipmentPool> { _equipmentPool };
        _encounterTypeProvider.Setup(encounterProvider => encounterProvider.GetEncounterType())
            .Returns(EncounterType.Siege);
        _troopSiegeEquipmentProvider.Setup(troopSiegeEquipmentProvider =>
                troopSiegeEquipmentProvider.GetSiegeTroopEquipmentPools(TroopId))
            .Returns(equipmentPools);

        var equipment = _getEquipmentPoolsUtil.GetEquipmentPools(TroopId);

        Assert.That(equipment, Is.EqualTo(equipmentPools));
    }

    [Test]
    public void ReturnCivilianEquipmentPoolsWhenEncounterIsCivilian()
    {
        var equipmentPools = new List<Domain.EquipmentPool.Model.EquipmentPool> { _equipmentPool };
        _encounterTypeProvider.Setup(encounterProvider => encounterProvider.GetEncounterType())
            .Returns(EncounterType.Civilian);
        _troopCivilianEquipmentProvider.Setup(troopCivilianEquipmentProvider =>
                troopCivilianEquipmentProvider.GetCivilianTroopEquipmentPools(TroopId))
            .Returns(equipmentPools);

        var equipment = _getEquipmentPoolsUtil.GetEquipmentPools(TroopId);

        Assert.That(equipment, Is.EqualTo(equipmentPools));
    }

    [Test]
    public void LogWhenNoBattleEquipment()
    {
        var equipmentPools = new List<Domain.EquipmentPool.Model.EquipmentPool>();
        _encounterTypeProvider.Setup(encounterProvider => encounterProvider.GetEncounterType())
            .Returns(EncounterType.Battle);
        _troopBattleEquipmentProvider.Setup(listBattleEquipment => listBattleEquipment
                .GetBattleTroopEquipmentPools(TroopId))
            .Returns(equipmentPools);

        _getEquipmentPoolsUtil.GetEquipmentPools(TroopId);

        _logger.Verify(
            logger => logger.Warn(
                It.Is<string>(m =>
                    m.Equals($"No equipment found for troop '{TroopId}' in {EncounterType.Battle} encounter.")),
                null),
            Times.Once);
    }

    [Test]
    public void LogWhenNoCivilianEquipment()
    {
        var equipmentPools = new List<Domain.EquipmentPool.Model.EquipmentPool>();
        _encounterTypeProvider.Setup(encounterProvider => encounterProvider.GetEncounterType())
            .Returns(EncounterType.Civilian);
        _troopCivilianEquipmentProvider.Setup(listBattleEquipment => listBattleEquipment
                .GetCivilianTroopEquipmentPools(TroopId))
            .Returns(equipmentPools);

        _getEquipmentPoolsUtil.GetEquipmentPools(TroopId);

        _logger.Verify(
            logger => logger.Warn(
                It.Is<string>(m =>
                    m.Equals($"No equipment found for troop '{TroopId}' in {EncounterType.Civilian} encounter.")),
                null),
            Times.Once);
    }

    [Test]
    public void LogWhenNoSiegeEquipment()
    {
        var equipmentPools = new List<Domain.EquipmentPool.Model.EquipmentPool>();
        _encounterTypeProvider.Setup(encounterProvider => encounterProvider.GetEncounterType())
            .Returns(EncounterType.Siege);
        _troopSiegeEquipmentProvider.Setup(listBattleEquipment => listBattleEquipment
                .GetSiegeTroopEquipmentPools(TroopId))
            .Returns(equipmentPools);

        _getEquipmentPoolsUtil.GetEquipmentPools(TroopId);

        _logger.Verify(
            logger => logger.Warn(
                It.Is<string>(m =>
                    m.Equals($"No equipment found for troop '{TroopId}' in {EncounterType.Siege} encounter.")),
                null),
            Times.Once);
    }

    private static Domain.EquipmentPool.Model.EquipmentPool CreateEquipmentPool()
    {
        return new Domain.EquipmentPool.Model.EquipmentPool(new List<Equipment> { CreateEquipmentNode() }, 0);
    }

    private static Equipment CreateEquipmentNode()
    {
        return new Equipment(new List<EquipmentSlot> { new("item", "EquipmentId2") });
    }
}