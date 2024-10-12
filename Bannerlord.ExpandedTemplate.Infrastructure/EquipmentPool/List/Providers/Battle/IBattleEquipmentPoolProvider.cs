using System.Collections.Generic;

namespace Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Battle
{
    public interface IBattleEquipmentPoolProvider
    {
        IDictionary<string, IList<Domain.EquipmentPool.Model.EquipmentPool>> GetBattleEquipmentByCharacterAndPool();
    }
}