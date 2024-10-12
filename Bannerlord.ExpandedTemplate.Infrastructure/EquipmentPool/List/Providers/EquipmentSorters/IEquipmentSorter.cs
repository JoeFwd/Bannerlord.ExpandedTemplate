using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentSorters
{
    public interface IEquipmentSorter
    {
        IList<Domain.EquipmentPool.Model.EquipmentPool> GetEquipmentPools();
    }
}
