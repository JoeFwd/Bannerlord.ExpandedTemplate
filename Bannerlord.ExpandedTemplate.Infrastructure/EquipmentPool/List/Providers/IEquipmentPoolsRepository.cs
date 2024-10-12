using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers
{
    public interface IEquipmentPoolsRepository
    {
        IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>> GetEquipmentPoolsById();
    }
}