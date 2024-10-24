using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Battle;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Civilian;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.Siege;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using Bannerlord.ExpandedTemplate.Infrastructure.Logging;
using Bannerlord.ExpandedTemplate.Integration.Caching;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.EquipmentPools.Mappers;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.EquipmentPools.Providers;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic;
using Bannerlord.ExpandedTemplate.Integration.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.ExpandedTemplate.Integration
{
    public class SubModule : MBSubModuleBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ICacheProvider _cacheProvider;

        private ForceCivilianEquipmentSetter _forceCivilianEquipmentSetter;
        private MissionSpawnEquipmentPoolSetter _missionSpawnEquipmentPoolSetter;

        public SubModule()
        {
            _cacheProvider = new CacheCampaignBehaviour();
            _loggerFactory = new ConsoleLoggerFactory();
        }

        public SubModule(ILoggerFactory loggerFactory) : this()
        {
            _loggerFactory = loggerFactory;
        }

        public override void OnBeforeMissionBehaviorInitialize(Mission mission)
        {
            base.OnBeforeMissionBehaviorInitialize(mission);

            AddEquipmentSpawnMissionBehaviour(mission);
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            _cacheProvider.InvalidateCache();

            if (game.GameType is not Campaign || starterObject is not CampaignGameStarter campaignGameStarter) return;

            HandleEquipmentSpawnDependencies();

            campaignGameStarter.AddBehavior(_cacheProvider as CampaignBehaviorBase);
        }

        #region GetEquipmentSpawn

        private void HandleEquipmentSpawnDependencies()
        {
            var xmlProcessor = new MergedModulesXmlProcessor(_loggerFactory, _cacheProvider);
            var npcCharacterRepository = new NpcCharacterRepository(xmlProcessor, _cacheProvider, _loggerFactory);
            var equipmentPoolRoster = new EquipmentSetMapper();
            var equipmentRosterRepository = new EquipmentRosterRepository(xmlProcessor, _cacheProvider, _loggerFactory);
            var npcCharacterMapper =
                new NpcCharacterMapper(equipmentRosterRepository, equipmentPoolRoster, _loggerFactory);
            var characterEquipmentPoolRepository =
                new NpcCharacterEquipmentPoolsProvider(npcCharacterRepository, npcCharacterMapper);
            var civilianEquipmentRepository =
                new CivilianEquipmentPoolProvider(_loggerFactory, _cacheProvider, characterEquipmentPoolRepository);
            var siegeEquipmentRepository =
                new SiegeEquipmentPoolProvider(_loggerFactory, _cacheProvider, characterEquipmentPoolRepository);
            var battleEquipmentRepository =
                new BattleEquipmentPoolProvider(_loggerFactory, _cacheProvider, siegeEquipmentRepository,
                    civilianEquipmentRepository,
                    characterEquipmentPoolRepository);
            var troopBattleEquipmentProvider =
                new TroopBattleEquipmentProvider(_loggerFactory, battleEquipmentRepository, _cacheProvider);
            var troopSiegeEquipmentProvider =
                new TroopSiegeEquipmentProvider(_loggerFactory, siegeEquipmentRepository, _cacheProvider);
            var troopCivilianEquipmentProvider =
                new TroopCivilianEquipmentProvider(_loggerFactory, civilianEquipmentRepository, _cacheProvider);
            var encounterTypeProvider = new EncounterTypeProvider();

            var random = new Random();
            var equipmentPicker = new EquipmentPoolPoolPicker(random);

            var equipmentMapper = new EquipmentMapper(MBObjectManager.Instance, _loggerFactory);
            var equipmentPoolMapper =
                new EquipmentPoolsMapper(equipmentMapper, _loggerFactory);
            var getEquipmentPool = new GetEquipmentPool(encounterTypeProvider, troopBattleEquipmentProvider,
                troopSiegeEquipmentProvider, troopCivilianEquipmentProvider, equipmentPicker, _loggerFactory);
            var getEquipment = new GetEquipment(random);

            _forceCivilianEquipmentSetter = new ForceCivilianEquipmentSetter();
            _missionSpawnEquipmentPoolSetter =
                new MissionSpawnEquipmentPoolSetter(getEquipmentPool, getEquipment, equipmentPoolMapper,
                    equipmentMapper,
                    _loggerFactory);
        }

        private void AddEquipmentSpawnMissionBehaviour(Mission mission)
        {
            mission.AddMissionBehavior(_forceCivilianEquipmentSetter);
            mission.AddMissionBehavior(_missionSpawnEquipmentPoolSetter);
        }
        #endregion

        public void Inject()
        {
            Module.CurrentModule.SubModules.Add(this);
        }
    }
}
