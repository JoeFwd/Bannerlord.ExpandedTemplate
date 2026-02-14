using System;
using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
using Bannerlord.ExpandedTemplate.Infrastructure.Logging;
using Bannerlord.ExpandedTemplate.Integration.Module;
using Harmony.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.ExpandedTemplate.Integration
{
    public class SubModule : MBSubModuleBase
    {
        private IServiceProvider _serviceProvider;
        private IServiceCollection _serviceCollection;

        private void InitialiseServiceCollection()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddSingleton<ILoggerFactory, ConsoleLoggerFactory>();
            ServiceConfiguration.ConfigureServices(_serviceCollection);
            _serviceCollection.AddHarmonyPatching();
        }
        
        public SubModule()
        {
            InitialiseServiceCollection();
            _serviceCollection.AddSingleton<ILoggerFactory, ConsoleLoggerFactory>();
        }

        public SubModule(ILoggerFactory loggerFactory)
        {
            InitialiseServiceCollection();
            _serviceCollection.AddSingleton(loggerFactory);
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            if (game.GameType is not Campaign || starterObject is not CampaignGameStarter campaignGameStarter) return;

            ServiceConfiguration.RegisterGameDependencies(_serviceCollection);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            _serviceProvider.GetService<IHarmonyPatcher>().ApplyPatches();

            var behaviors = _serviceProvider.GetServices<CampaignBehaviorBase>();
            foreach (var behavior in behaviors) campaignGameStarter.AddBehavior(behavior);
        }

        public void Inject()
        {
            // uncomment for v1.3
            // _serviceCollection.AddSingleton<SubModuleInjector>();
            // _serviceCollection.BuildServiceProvider().GetService<SubModuleInjector>().Inject();
            TaleWorlds.MountAndBlade.Module.CurrentModule.SubModules.Add(this);
        }
    }
}
