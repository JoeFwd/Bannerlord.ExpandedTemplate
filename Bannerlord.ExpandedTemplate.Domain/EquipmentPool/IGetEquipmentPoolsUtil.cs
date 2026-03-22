using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Domain.EquipmentPool
{
    /// <summary>
    /// Interface for retrieving multiple equipment pools for comparison purposes
    /// </summary>
    public interface IGetEquipmentPoolsUtil
    {
        /// <summary>
        /// Get all equipment pools for a troop across all encounter types
        /// </summary>
        /// <param name="troopId">The troop identifier</param>
        /// <returns>List of equipment pools for the troop</returns>
        IList<Model.EquipmentPool> GetEquipmentPools(string troopId);
    }
}