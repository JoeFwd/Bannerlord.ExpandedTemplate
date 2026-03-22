using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool
{
    public class GetEquipmentPool : IGetEquipmentPool
    {
        private readonly IGetEquipmentPoolsUtil _getEquipmentPoolsUtil;
        private readonly IEquipmentPoolPicker _equipmentPoolPicker;

        public GetEquipmentPool(IGetEquipmentPoolsUtil getEquipmentPoolsUtil,
            IEquipmentPoolPicker equipmentPoolPicker)
        {
            _getEquipmentPoolsUtil = getEquipmentPoolsUtil;
            _equipmentPoolPicker = equipmentPoolPicker;
        }

        public Model.EquipmentPool GetTroopEquipmentPool(string troopId)
        {
            return _equipmentPoolPicker.PickEquipmentPool(_getEquipmentPoolsUtil.GetEquipmentPools(troopId));
        }
    }
}