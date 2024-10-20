using System.Collections.Generic;
using System.Reflection;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.EquipmentPools.Mappers;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic
{
    public class MissionSpawnEquipmentPoolSetter : TaleWorlds.MountAndBlade.MissionLogic
    {
        private readonly FieldInfo? _equipmentRosterField =
            typeof(BasicCharacterObject).GetField("_equipmentRoster", BindingFlags.NonPublic | BindingFlags.Instance)!;

        private readonly IGetEquipmentPool _getEquipmentPool;
        private readonly IGetEquipment _getEquipment;
        private readonly EquipmentPoolsMapper _equipmentPoolsMapper;
        private readonly EquipmentMapper _equipmentMapper;
        private readonly ILogger _logger;

        private readonly Dictionary<string, MBEquipmentRoster> _nativeEquipmentPools = new();

        public MissionSpawnEquipmentPoolSetter(IGetEquipmentPool getEquipmentPool, IGetEquipment getEquipment,
            EquipmentPoolsMapper equipmentPoolsMapper, EquipmentMapper equipmentMapper, ILoggerFactory loggerFactory)
        {
            _getEquipmentPool = getEquipmentPool;
            _getEquipment = getEquipment;
            _equipmentPoolsMapper = equipmentPoolsMapper;
            _equipmentMapper = equipmentMapper;
            _logger = loggerFactory.CreateLogger<MissionSpawnEquipmentPoolSetter>();
            

            if (_equipmentRosterField is null || _equipmentRosterField.FieldType != typeof(MBEquipmentRoster))
                _logger.Error(
                        "BasicCharacterObject's _mbEquipmentRoster field could not be found preventing equipment pool override in friendly missions");
        }

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();

            _nativeEquipmentPools.Clear();
        }

        public override void OnAgentCreated(Agent agent)
        {
            if (_equipmentRosterField is null) return;
            if (!CanOverrideEquipment(agent)) return;

            base.OnAgentCreated(agent);

            var equipmentRoster = (MBEquipmentRoster)_equipmentRosterField.GetValue(agent.Character);

            string id = agent.Character.StringId;
            if (agent.Character is CharacterObject characterObject)
                id = characterObject.OriginalCharacter?.StringId ?? id;

            _nativeEquipmentPools[agent.Character.StringId] = equipmentRoster;

            var equipmentPool = _getEquipmentPool.GetTroopEquipmentPool(id);
            if (equipmentPool.IsEmpty())
                equipmentPool = _getEquipmentPool.GetTroopEquipmentPool(equipmentRoster.StringId);


            if (agent.IsHero)
            {
                var equipment = _getEquipment.GetEquipmentFromEquipmentPool(equipmentPool);
                if (equipment is null)
                    OverrideHeroEquipment(agent, new Equipment());
                else
                    OverrideHeroEquipment(agent,
                        _equipmentMapper.Map(equipment, equipmentRoster));
            }
            else
            {
                OverrideTroopEquipment(agent,
                    _equipmentPoolsMapper.MapEquipmentPool(equipmentPool, equipmentRoster));
            }
        }

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            if (_equipmentRosterField is null) return;
            if (!CanOverrideEquipment(agent)) return;

            base.OnAgentBuild(agent, banner);

            OverrideTroopEquipment(agent, _nativeEquipmentPools[agent.Character.StringId]);
        }

        private bool CanOverrideEquipment(IAgent agent)
        {
            return (Mission.Current?.GetMissionBehavior<MissionAgentHandler>() is not null ||
                    Mission.Current?.GetMissionBehavior<IMissionAgentSpawnLogic>() is not null) &&
                   agent?.Character is not null &&
                   !(Clan.PlayerClan?.Heroes?.Exists(
                       hero => hero?.StringId is not null && agent?.Character?.StringId == hero.StringId) ?? true);
        }

        private void OverrideTroopEquipment(IAgent agent, MBEquipmentRoster equipmentPool)
        {
            _equipmentRosterField?.SetValue(agent.Character, equipmentPool);
        }

        private void OverrideHeroEquipment(IAgent agent, Equipment? equipment)
        {
            if (agent.Character?.Equipment is null)
            {
                _logger.Error(
                    "Expected a hero Agent to have a non-nullable Character field with a non-nullable Equipment field");
                return;
            }

            if (equipment is null)
            {
                _logger.Error(
                    $"Could find any equipment for ${agent.Character.StringId}");
                return;
            }

            agent.Character.Equipment.FillFrom(equipment);
        }
    }
}