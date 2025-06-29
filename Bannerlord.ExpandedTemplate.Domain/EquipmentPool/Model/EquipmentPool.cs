using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model
{
    public class EquipmentPool
    {
        private readonly int _poolId;
        private readonly IList<Equipment> _equipment;

        public EquipmentPool(IList<Equipment> equipment, int poolId)
        {
            _equipment = equipment;
            _poolId = poolId;
        }

        public IList<Equipment> GetEquipmentLoadouts()
        {
            return _equipment;
        }

        public int GetPoolId()
        {
            return _poolId;
        }

        public bool IsEmpty()
        {
            return _equipment.Count == 0;
        }

        protected bool Equals(EquipmentPool other)
        {
            return _poolId == other._poolId && _equipment.SequenceEqual(other._equipment);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EquipmentPool)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 17;
            hashCode = (hashCode * 397) ^ _poolId.GetHashCode();
            foreach (var equipment in _equipment) hashCode = hashCode * 31 + equipment.GetHashCode();
            return hashCode;
        }
    }
}
