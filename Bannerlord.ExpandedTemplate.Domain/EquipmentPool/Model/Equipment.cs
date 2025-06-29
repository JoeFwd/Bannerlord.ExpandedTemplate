using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model
{
    public record Equipment
    {
        private readonly IList<EquipmentSlot> _equipmentSlots;

        public Equipment(IList<EquipmentSlot> equipmentSlots)
        {
            _equipmentSlots = equipmentSlots;
        }

        public IList<EquipmentSlot> GetEquipmentSlots()
        {
            return _equipmentSlots;
        }

        public virtual bool Equals(Equipment? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _equipmentSlots.SequenceEqual(other._equipmentSlots);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var item in _equipmentSlots) hash = hash * 31 + item.GetHashCode();
            return hash;
        }
    }
}
