using Bannerlord.ExpandedTemplate.Domain.EquipmentPool;
using Bannerlord.ExpandedTemplate.Domain.EquipmentPool.Util;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Caching;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Battle;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Civilian;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.Get.Siege;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Mappers;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentPool;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Battle;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Civilian;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Pool;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Providers.EquipmentRosters.Siege;
using Bannerlord.ExpandedTemplate.Infrastructure.EquipmentPool.List.Repositories;
using Bannerlord.ExpandedTemplate.Infrastructure.Logging;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool.List.Repositories.Spi;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool.Spi;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.Mappers;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic.EquipmentSetters;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.ExpandedTemplate.Integration
{
    public class SubModule : MBSubModuleBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ICachingProvider _cachingProvider;

        private EquipmentPoolsProvider _civilianEquipmentPoolsProvider;
        private EquipmentPoolsProvider _siegeEquipmentPoolsProvider;
        private EquipmentPoolsProvider _battleEquipmentPoolsProvider;

        private EquipmentSetterMissionLogic? _equipmentSetterMissionLogic;

        public SubModule()
        {
            _cachingProvider = new InMemoryCacheProvider();
            _loggerFactory = new ConsoleLoggerFactory();
            
            InstantiateEquipmentPoolProviders();
        }

        public SubModule(ILoggerFactory loggerFactory) : this()
        {
            _loggerFactory = loggerFactory;

            // Override built dependencies with default logger, everything would be cleaner with a real DI container though
            InstantiateEquipmentPoolProviders();
        }

        public override void OnBeforeMissionBehaviorInitialize(Mission mission)
        {
            base.OnBeforeMissionBehaviorInitialize(mission);

            AddEquipmentSpawnMissionBehaviour(mission);
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            if (game.GameType is not Campaign || starterObject is not CampaignGameStarter campaignGameStarter) return;
            
            campaignGameStarter.AddBehavior(new CampaignLoadEquipmentPoolHandler(_cachingProvider as ICacheInvalidator,
                _battleEquipmentPoolsProvider, _civilianEquipmentPoolsProvider, _siegeEquipmentPoolsProvider));
        }

        private void InstantiateEquipmentPoolProviders()
        {
            IXmlProcessor xmlProcessor = new MergedModulesXmlProcessor(_loggerFactory, _cachingProvider);
            var npcCharacterRepository = new NpcCharacterRepository(xmlProcessor, _cachingProvider, _loggerFactory);
            var equipmentPoolRoster = new EquipmentSetMapper();
            var equipmentRosterRepository =
                new EquipmentRosterRepository(xmlProcessor, _cachingProvider, _loggerFactory);
            var npcCharacterMapper =
                new NpcCharacterMapper(equipmentRosterRepository, equipmentPoolRoster, _loggerFactory);

            var npcCharacterWithResolvedEquipmentProvider =
                new NpcCharacterWithResolvedEquipmentProvider(npcCharacterRepository, npcCharacterMapper,
                    _loggerFactory);

            var siegeEquipmentRostersProvider =
                new SiegeEquipmentRosterProvider(npcCharacterWithResolvedEquipmentProvider);
            var civilianEquipmentRostersProvider =
                new CivilianEquipmentRosterProvider(npcCharacterWithResolvedEquipmentProvider);
            var battleEquipmentRosterProvider = new BattleEquipmentRosterProvider(siegeEquipmentRostersProvider,
                civilianEquipmentRostersProvider, npcCharacterWithResolvedEquipmentProvider);
            var poolEquipmentRostersProvider =
                new PoolEquipmentRosterProvider(npcCharacterWithResolvedEquipmentProvider);

            var equipmentRosterMapper = new EquipmentRosterMapper();

            _battleEquipmentPoolsProvider = new EquipmentPoolsProvider(battleEquipmentRosterProvider,
                poolEquipmentRostersProvider, equipmentRosterMapper, _cachingProvider);
            _siegeEquipmentPoolsProvider = new EquipmentPoolsProvider(siegeEquipmentRostersProvider,
                poolEquipmentRostersProvider, equipmentRosterMapper, _cachingProvider);
            _civilianEquipmentPoolsProvider = new EquipmentPoolsProvider(civilianEquipmentRostersProvider,
                poolEquipmentRostersProvider, equipmentRosterMapper, _cachingProvider);
        }

        private EquipmentSetterMissionLogic InstantiateSpawnEquipmentMissionLogic()
        {
            var troopBattleEquipmentPoolProvider =
                new TroopBattleEquipmentPoolProvider(_loggerFactory, _battleEquipmentPoolsProvider);
            var troopSiegeEquipmentPoolProvider =
                new TroopSiegeEquipmentPoolProvider(_loggerFactory, _siegeEquipmentPoolsProvider);
            var troopCivilianEquipmentPoolProvider =
                new TroopCivilianEquipmentPoolProvider(_loggerFactory, _civilianEquipmentPoolsProvider);

            var encounterTypeProvider = new EncounterTypeProvider();

            var random = new Random();
            var equipmentPicker = new EquipmentPoolPoolPicker(random);

            var getEquipmentPool = new GetEquipmentPool(encounterTypeProvider, troopBattleEquipmentPoolProvider,
                troopSiegeEquipmentPoolProvider, troopCivilianEquipmentPoolProvider, equipmentPicker, _loggerFactory);
            var getEquipment = new GetEquipment(random);

            var equipmentMapper = new EquipmentMapper(MBObjectManager.Instance, _loggerFactory);
            var equipmentPoolMapper = new EquipmentPoolsMapper(equipmentMapper, _loggerFactory);
            var characterEquipmentRosterReference = new CharacterEquipmentRosterReference(_loggerFactory);
            var heroEquipmentSetter = new HeroEquipmentSetter(getEquipment, equipmentMapper,
                characterEquipmentRosterReference, _loggerFactory);
            var troopEquipmentPoolSetter =
                new TroopEquipmentPoolSetter(equipmentPoolMapper, characterEquipmentRosterReference);

            return new EquipmentSetterMissionLogic(heroEquipmentSetter,
                troopEquipmentPoolSetter, getEquipmentPool, characterEquipmentRosterReference, _loggerFactory);
        }

        private void AddEquipmentSpawnMissionBehaviour(Mission mission)
        {
            _equipmentSetterMissionLogic ??= InstantiateSpawnEquipmentMissionLogic();
            mission.AddMissionBehavior(_equipmentSetterMissionLogic);
        }

        public void Inject()
        {
            Module.CurrentModule.SubModules.Add(this);
        }
    }
}
