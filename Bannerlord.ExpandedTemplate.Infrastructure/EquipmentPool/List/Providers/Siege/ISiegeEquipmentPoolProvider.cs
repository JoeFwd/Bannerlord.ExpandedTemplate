using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Siege
{
    public interface ISiegeEquipmentPoolProvider
    {
        IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>> GetSiegeEquipmentByCharacterAndPool();
    }
}