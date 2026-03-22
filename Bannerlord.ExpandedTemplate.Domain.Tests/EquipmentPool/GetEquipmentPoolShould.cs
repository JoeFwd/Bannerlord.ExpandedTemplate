using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Port;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Moq;
using NUnit.Framework;

namespace Bannerlord.ExpandedTemplate.Domain.Tests.EquipmentPool;

public class GetEquipmentPoolShould
{
    private const string TroopId = "unrelevant troop id";
    private readonly Domain.EquipmentPool.Model.EquipmentPool _equipmentPool = CreateEquipmentPool();
    private Mock<IGetEquipmentPoolsUtil> _getEquipmentPoolsUtil;
    private Mock<IEquipmentPoolPicker> _equipmentPoolPicker;
    private GetEquipmentPool _getEquipmentPool;

    [SetUp]
    public void Setup()
    {
        _getEquipmentPoolsUtil = new Mock<IGetEquipmentPoolsUtil>();
        _equipmentPoolPicker = new Mock<IEquipmentPoolPicker>();
        _getEquipmentPool = new GetEquipmentPool(_getEquipmentPoolsUtil.Object, _equipmentPoolPicker.Object);
    }

    [Test]
    public void ShouldPickEquipmentPoolFromAllPools()
    {
        var equipmentPools = new List<Domain.EquipmentPool.Model.EquipmentPool> { _equipmentPool };
        _getEquipmentPoolsUtil.Setup(util => util.GetEquipmentPools(TroopId))
            .Returns(equipmentPools);
        _equipmentPoolPicker.Setup(picker => picker.PickEquipmentPool(equipmentPools))
            .Returns(_equipmentPool);

        var result = _getEquipmentPool.GetTroopEquipmentPool(TroopId);

        Assert.That(result, Is.EqualTo(_equipmentPool));
        _getEquipmentPoolsUtil.Verify(util => util.GetEquipmentPools(TroopId), Times.Once);
        _equipmentPoolPicker.Verify(picker => picker.PickEquipmentPool(equipmentPools), Times.Once);
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