using System.Collections.Generic;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Moq;
using NUnit.Framework;
using EncounterType = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model.EncounterType;
using EquipmentPoolModel = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model.EquipmentPool;
using EquipmentModel = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model.Equipment;
using EquipmentSlotModel = Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model.EquipmentSlot;

namespace Bannerlord.ExpandedTemplate.Domain.Tests.EquipmentPool;

public class GetEquipmentPoolsUtilShould
{
    private const string TroopId = "unrelevant troop id";
    private readonly EquipmentPoolModel _equipmentPool = CreateEquipmentPool();
    private Mock<IEncounterTypeProvider> _encounterTypeProvider = null!;
    private GetEquipmentPoolsUtil _getEquipmentPoolsUtil = null!;
    private Mock<ILogger> _logger = null!;
    private Mock<ITroopBattleEquipmentProvider> _troopBattleEquipmentProvider = null!;
    private Mock<ITroopCivilianEquipmentProvider> _troopCivilianEquipmentProvider = null!;
    private Mock<ITroopSiegeEquipmentProvider> _troopSiegeEquipmentProvider = null!;
    private Mock<ITroopStealthEquipmentProvider> _troopStealthEquipmentProvider = null!;

    [SetUp]
    public void SetUp()
    {
        _troopBattleEquipmentProvider = new Mock<ITroopBattleEquipmentProvider>();
        _troopSiegeEquipmentProvider = new Mock<ITroopSiegeEquipmentProvider>();
        _troopCivilianEquipmentProvider = new Mock<ITroopCivilianEquipmentProvider>();
        _troopStealthEquipmentProvider = new Mock<ITroopStealthEquipmentProvider>();
        _encounterTypeProvider = new Mock<IEncounterTypeProvider>();
        _logger = new Mock<ILogger>();
        var loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory.Setup(factory => factory.CreateLogger<GetEquipmentPoolsUtil>()).Returns(_logger.Object);
        _getEquipmentPoolsUtil = new GetEquipmentPoolsUtil(_encounterTypeProvider.Object,
            _troopBattleEquipmentProvider.Object, _troopSiegeEquipmentProvider.Object,
            _troopCivilianEquipmentProvider.Object, _troopStealthEquipmentProvider.Object, loggerFactory.Object);
    }

    [TestCase(EncounterType.Battle)]
    [TestCase(EncounterType.Siege)]
    [TestCase(EncounterType.Civilian)]
    [TestCase(EncounterType.Stealth)]
    public void ReturnEquipmentPoolsForTheEncounterType(EncounterType encounterType)
    {
        IList<EquipmentPoolModel> expected = new List<EquipmentPoolModel> { _equipmentPool };
        SetUpEncounter(encounterType, expected);

        IList<EquipmentPoolModel> actual = _getEquipmentPoolsUtil.GetEquipmentPools(TroopId);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase(EncounterType.Battle)]
    [TestCase(EncounterType.Siege)]
    [TestCase(EncounterType.Civilian)]
    [TestCase(EncounterType.Stealth)]
    public void LogWhenTheEncounterTypeHasNoEquipment(EncounterType encounterType)
    {
        SetUpEncounter(encounterType, new List<EquipmentPoolModel>());

        _getEquipmentPoolsUtil.GetEquipmentPools(TroopId);

        _logger.Verify(logger => logger.Warn(
                It.Is<string>(message => message.Equals($"No equipment found for troop '{TroopId}' in {encounterType} encounter.")),
                null),
            Times.Once);
    }

    private void SetUpEncounter(EncounterType encounterType, IList<EquipmentPoolModel> equipmentPools)
    {
        _encounterTypeProvider.Setup(provider => provider.GetEncounterType()).Returns(encounterType);
        switch (encounterType)
        {
            case EncounterType.Battle:
                _troopBattleEquipmentProvider.Setup(provider => provider.GetBattleTroopEquipmentPools(TroopId))
                    .Returns(equipmentPools);
                break;
            case EncounterType.Siege:
                _troopSiegeEquipmentProvider.Setup(provider => provider.GetSiegeTroopEquipmentPools(TroopId))
                    .Returns(equipmentPools);
                break;
            case EncounterType.Civilian:
                _troopCivilianEquipmentProvider.Setup(provider => provider.GetCivilianTroopEquipmentPools(TroopId))
                    .Returns(equipmentPools);
                break;
            case EncounterType.Stealth:
                _troopStealthEquipmentProvider.Setup(provider => provider.GetStealthTroopEquipmentPools(TroopId))
                    .Returns(equipmentPools);
                break;
            default:
                Assert.Fail($"Unsupported encounter type {encounterType}.");
                break;
        }
    }

    private static EquipmentPoolModel CreateEquipmentPool()
    {
        return new EquipmentPoolModel(new List<EquipmentModel>
        {
            new(new List<EquipmentSlotModel> { new("item", "EquipmentId2") })
        }, 0);
    }
}
