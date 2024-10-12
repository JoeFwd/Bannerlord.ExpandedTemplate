﻿using System.Xml.Linq;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Model
{
    public class Equipment
    {
        private readonly XNode _equipmentNode;
        private readonly XNodeEqualityComparer _xNodeComparer = new ();

        public Equipment(XNode equipmentNode)
        {
            _equipmentNode = equipmentNode;
        }

        public XNode GetEquipmentNode()
        {
            return _equipmentNode;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Equipment)obj);
        }

        public override int GetHashCode()
        {
            return _xNodeComparer.GetHashCode(_equipmentNode);
        }

        private bool Equals(Equipment other)
        {
            return _equipmentNode.ToString().Equals(other._equipmentNode.ToString());
        }

        public override string ToString()
        {
            return _equipmentNode.ToString();
        }
    }
}
