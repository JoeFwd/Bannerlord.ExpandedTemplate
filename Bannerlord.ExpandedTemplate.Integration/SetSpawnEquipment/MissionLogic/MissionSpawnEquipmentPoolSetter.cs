﻿using System.Collections.Generic;
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
        private readonly FieldInfo _equipmentRosterField =
            typeof(BasicCharacterObject).GetField("_equipmentRoster", BindingFlags.NonPublic | BindingFlags.Instance)!;

        private readonly IGetEquipmentPool _getEquipmentPool;
        private readonly EquipmentPoolsMapper _equipmentPoolsMapper;

        private readonly Dictionary<string, MBEquipmentRoster> _nativeEquipmentPools = new();

        public MissionSpawnEquipmentPoolSetter(IGetEquipmentPool getEquipmentPool,
            EquipmentPoolsMapper equipmentPoolsMapper, ILoggerFactory loggerFactory)
        {
            _getEquipmentPool = getEquipmentPool;
            _equipmentPoolsMapper = equipmentPoolsMapper;

            if (_equipmentRosterField is null || _equipmentRosterField.FieldType != typeof(MBEquipmentRoster))
                loggerFactory.CreateLogger<MissionSpawnEquipmentPoolSetter>()
                    .Error(
                        "BasicCharacterObject's _mbEquipmentRoster field could not be found preventing equipment pool override in friendly missions");
        }

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();

            _nativeEquipmentPools.Clear();
        }

        public override void OnAgentCreated(Agent agent)
        {
            if (!CanOverrideEquipment(agent))
                return;

            base.OnAgentCreated(agent);

            var equipmentRoster =
                (MBEquipmentRoster)_equipmentRosterField.GetValue(agent.Character);

            string id = agent.Character.StringId;
            if (agent.Character is CharacterObject characterObject)
                id = characterObject.OriginalCharacter?.StringId ?? id;

            _nativeEquipmentPools[agent.Character.StringId] = equipmentRoster;

            var equipmentPool = _getEquipmentPool.GetTroopEquipmentPool(id);
            if (equipmentPool.IsEmpty())
                equipmentPool = _getEquipmentPool.GetTroopEquipmentPool(equipmentRoster.StringId);

            OverrideTroopEquipment(agent,
                _equipmentPoolsMapper.MapEquipmentPool(equipmentPool, equipmentRoster.StringId));
        }

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            if (!CanOverrideEquipment(agent))
                return;

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
            if (agent.Character.IsHero)
                agent.Character.Equipment.FillFrom(equipmentPool.AllEquipments.Count > 0
                    ? equipmentPool.AllEquipments[0]
                    : new Equipment());
            else
                _equipmentRosterField.SetValue(agent.Character, equipmentPool);
        }
    }
}