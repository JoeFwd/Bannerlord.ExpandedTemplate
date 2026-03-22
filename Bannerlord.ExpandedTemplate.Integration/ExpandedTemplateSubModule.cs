using System;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Integration.EquipmentPool;
using Bannerlord.ExpandedTemplate.Integration.Module;
using Harmony.DependencyInjection;
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
        private readonly ServiceCollection _services;

        public ExpandedTemplateSubModule()
        {
            _services = new ServiceCollection();
            ServiceConfiguration.ConfigureServices(_services);
            _services.AddHarmonyPatching();
            _serviceProvider = _services.BuildServiceProvider();
            _subModuleInjector = _serviceProvider.GetRequiredService<SubModuleInjector>();
        }

        public ExpandedTemplateSubModule(ILoggerFactory loggerFactory)
        {
            _services = new ServiceCollection();
            _services.AddSingleton(loggerFactory);
            ServiceConfiguration.ConfigureServices(_services);
            _services.AddHarmonyPatching();
            _serviceProvider = _services.BuildServiceProvider();
            _subModuleInjector = _serviceProvider.GetRequiredService<SubModuleInjector>();
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            if (game.GameType is not Campaign || starterObject is not CampaignGameStarter campaignGameStarter) return;

            ServiceConfiguration.RegisterGameDependencies(_services);
            _serviceProvider = _services.BuildServiceProvider();
            _serviceProvider.GetService<IHarmonyPatcher>().ApplyPatches();

            var behaviors = _serviceProvider.GetServices<CampaignBehaviorBase>();
            foreach (var behavior in behaviors) campaignGameStarter.AddBehavior(behavior);
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
