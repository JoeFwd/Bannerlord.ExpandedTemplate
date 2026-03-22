using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Domain.Tests.EquipmentPool;

public class EquipmentComparisonShould
{
    [Test]
    public void ReturnFalse_WhenCurrentEquipmentIsNull()
    {
        // Arrange
        var equipmentPoolsMock = new Mock<IGetEquipmentPoolsUtil>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(factory => factory.CreateLogger<EquipmentComparison>())
            .Returns(loggerMock.Object);

        // Act
        var equipmentComparison = new EquipmentComparison(equipmentPoolsMock.Object, loggerFactoryMock.Object);
        var result = equipmentComparison.ShouldOverrideEquipment(null, "default_troop");

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void ReturnFalse_WhenEquipmentPoolIsEmpty()
    {
        // Arrange
        var equipmentPoolsMock = new Mock<IGetEquipmentPoolsUtil>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(factory => factory.CreateLogger<EquipmentComparison>())
            .Returns(loggerMock.Object);

        var emptyEquipmentPool = new Domain.EquipmentPool.Model.EquipmentPool(new List<Equipment>(), 0);
        equipmentPoolsMock.Setup(pools => pools.GetEquipmentPools("default_troop"))
            .Returns(new List<Domain.EquipmentPool.Model.EquipmentPool> { emptyEquipmentPool });

        // Act
        var equipmentComparison = new EquipmentComparison(equipmentPoolsMock.Object, loggerFactoryMock.Object);
        var result = equipmentComparison.ShouldOverrideEquipment(CreateTestEquipment(), "default_troop");

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void ReturnTrue_WhenCurrentEquipmentMatchesEquipmentInPool()
    {
        // Arrange
        var equipmentPoolsMock = new Mock<IGetEquipmentPoolsUtil>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(factory => factory.CreateLogger<EquipmentComparison>())
            .Returns(loggerMock.Object);

        var equipmentSlot = new EquipmentSlot("slot1", "itemId1");
        var equipmentInPool = new Equipment(new List<EquipmentSlot> { equipmentSlot });
        var equipmentPool = new Domain.EquipmentPool.Model.EquipmentPool(new List<Equipment> { equipmentInPool }, 0);
        equipmentPoolsMock.Setup(pools => pools.GetEquipmentPools("default_troop"))
            .Returns(new List<Domain.EquipmentPool.Model.EquipmentPool> { equipmentPool });

        var currentEquipment = new Equipment(new List<EquipmentSlot> { equipmentSlot });

        // Act
        var equipmentComparison = new EquipmentComparison(equipmentPoolsMock.Object, loggerFactoryMock.Object);
        var result = equipmentComparison.ShouldOverrideEquipment(currentEquipment, "default_troop");

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void ReturnFalse_WhenCurrentEquipmentDoesNotMatchEquipmentInPool()
    {
        // Arrange
        var equipmentPoolsMock = new Mock<IGetEquipmentPoolsUtil>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(factory => factory.CreateLogger<EquipmentComparison>())
            .Returns(loggerMock.Object);

        var equipmentSlotInPool = new EquipmentSlot("slot1", "itemId1");
        var equipmentSlotInCurrent = new EquipmentSlot("slot1", "itemId2"); // Different itemId
        var equipmentInPool = new Equipment(new List<EquipmentSlot> { equipmentSlotInPool });
        var equipmentPool = new Domain.EquipmentPool.Model.EquipmentPool(new List<Equipment> { equipmentInPool }, 0);
        equipmentPoolsMock.Setup(pools => pools.GetEquipmentPools("default_troop"))
            .Returns(new List<Domain.EquipmentPool.Model.EquipmentPool> { equipmentPool });

        var currentEquipment = new Equipment(new List<EquipmentSlot> { equipmentSlotInCurrent });

        // Act
        var equipmentComparison = new EquipmentComparison(equipmentPoolsMock.Object, loggerFactoryMock.Object);
        var result = equipmentComparison.ShouldOverrideEquipment(currentEquipment, "default_troop");

        // Assert
        Assert.IsFalse(result);
    }

    private Equipment CreateTestEquipment()
    {
        return new Equipment(new List<EquipmentSlot> { new("slot1", "itemId1") });
    }

    [Test]
    public void ReturnFalse_WhenExceptionOccursDuringComparison()
    {
        // Arrange
        var equipmentPoolsMock = new Mock<IGetEquipmentPoolsUtil>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        var loggerMock = new Mock<ILogger>();
        loggerFactoryMock.Setup(factory => factory.CreateLogger<EquipmentComparison>())
            .Returns(loggerMock.Object);

        var exception = new Exception("Test exception");
        equipmentPoolsMock.Setup(pools => pools.GetEquipmentPools("default_troop"))
            .Throws(exception);

        // Act
        var equipmentComparison = new EquipmentComparison(equipmentPoolsMock.Object, loggerFactoryMock.Object);
        var result = equipmentComparison.ShouldOverrideEquipment(CreateTestEquipment(), "default_troop");

        // Assert
        Assert.IsFalse(result);
        loggerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }
}