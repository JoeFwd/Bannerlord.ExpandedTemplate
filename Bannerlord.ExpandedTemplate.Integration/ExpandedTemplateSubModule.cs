using System;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool;
using Bannerlord.ExpandedTemplate.Integration.Module;
using Bannerlord.ExpandedTemplate.Integration.SetSpawnEquipment.MissionLogic;
using Microsoft.Extensions.DependencyInjection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.ExpandedTemplate.Integration
{
    public class ExpandedTemplateSubModule : MBSubModuleBase, IDisposable
    {
        private ServiceProvider _serviceProvider;
        private readonly SubModuleInjector _subModuleInjector;
        private EquipmentSetterMissionLogic? _equipmentSetterMissionLogic;
        private readonly ServiceCollection _services;
        private bool _gameServicesRegistered;
        
        public ExpandedTemplateSubModule()
        {
            _services = new ServiceCollection();
            ServiceConfiguration.ConfigureServices(_services);
            _serviceProvider = _services.BuildServiceProvider();
            _subModuleInjector = _serviceProvider.GetRequiredService<SubModuleInjector>();
            _gameServicesRegistered = false;
        }

        public ExpandedTemplateSubModule(ILoggerFactory loggerFactory)
        {
            _services = new ServiceCollection();
            _services.AddSingleton(loggerFactory);
            ServiceConfiguration.ConfigureServices(_services);
            _serviceProvider = _services.BuildServiceProvider();
            _subModuleInjector = _serviceProvider.GetRequiredService<SubModuleInjector>();
            _gameServicesRegistered = false;
        }
        
        private void RegisterGameDependenciesIfNeeded()
        {
            if (!_gameServicesRegistered && Game.Current?.ObjectManager != null)
            {
                // Register game-dependent services and rebuild the service provider
                ServiceConfiguration.RegisterGameDependencies(_services);
                _serviceProvider = _services.BuildServiceProvider();
                _gameServicesRegistered = true;
            }
        }

        public override void OnBeforeMissionBehaviorInitialize(Mission mission)
        {
            base.OnBeforeMissionBehaviorInitialize(mission);

            RegisterGameDependenciesIfNeeded();
            AddEquipmentSpawnMissionBehaviour(mission);
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            if (game.GameType is not Campaign || starterObject is not CampaignGameStarter campaignGameStarter) return;

            RegisterGameDependenciesIfNeeded();
            var campaignBehavior = _serviceProvider.GetRequiredService<CampaignLoadEquipmentPoolHandler>();
            campaignGameStarter.AddBehavior(campaignBehavior);
        }
        
        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            RegisterGameDependenciesIfNeeded();
        }

        private void AddEquipmentSpawnMissionBehaviour(Mission mission)
        {
            _equipmentSetterMissionLogic = _serviceProvider.GetRequiredService<EquipmentSetterMissionLogic>();
            mission.AddMissionBehavior(_equipmentSetterMissionLogic);
        }

        public void Inject()
        {
            _subModuleInjector.Inject();
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);

            Dispose();
        }

        public void Dispose()
        {
            if (_serviceProvider != null) _serviceProvider.Dispose();
        }
    }
}
